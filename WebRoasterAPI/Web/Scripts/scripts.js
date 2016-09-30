var currentTemp = -999;
var timerId = 0

var holdTemp = 300;

timerId = setInterval(function () {
    // Wait to show the main page until the temperature probe is online and reading data
    if (currentTemp != -999) {
        clearInterval(timerId);
        $("#main-page").removeClass("hidden");
        window.location.replace("#main-page");

        // Remove the #main-page ending from the url, so if the page is refreshed it adds the loader again
        if (typeof window.history.replaceState == 'function') {
            history.replaceState({}, '', window.location.href.slice(0, -10));
        }
    }

}, 1000);

$("#fan-state").on("change", function () {
    if ($(this).is(":checked")) {
        $.get('api/Fan/ChangeState/On');
    } else {
        $.get('api/Fan/ChangeState/Off');
    }
});

$("#heater-state").on("change", function () {
    if ($(this).is(":checked")) {
        $.get('api/Heater/ChangeState/On');
    } else {
        $.get('api/Heater/ChangeState/Off');
    }
});

$("#hold-state").on("change", function () {
    if ($(this).is(":checked")) {
        $("#fan-state").parent("div").addClass("ui-flipswitch-active");
        $.get('api/Fan/ChangeState/On');
        $("#fan-state").attr("disabled", "disabled");
        $("#fan-state").parent("div").addClass("ui-state-disabled");
        $("#fan-state").parent("div").addClass("mobile-flipswitch-disabled");

        $("#heater-state").parent("div").addClass("ui-flipswitch-active");
        $.get('api/Heater/ChangeState/On');
        $("#heater-state").attr("disabled", "disabled");
        $("#heater-state").parent("div").addClass("ui-state-disabled");
        $("#heater-state").parent("div").addClass("mobile-flipswitch-disabled");
    } else {
        $("#fan-state").removeAttr("disabled");
        $("#fan-state").parent("div").removeClass("ui-state-disabled");
        $("#fan-state").parent("div").removeClass("mobile-flipswitch-disabled");

        $.get('api/Heater/ChangeState/Off');
        $("#heater-state").parent("div").removeClass("ui-flipswitch-active");
        $("#heater-state").removeAttr("disabled");
        $("#heater-state").parent("div").removeClass("ui-state-disabled");
        $("#heater-state").parent("div").removeClass("mobile-flipswitch-disabled");
    }
});

var data = [{ "time": 0, "temperature": 0 }];

var startRun = false;
var startAt = new Date().getTime();

$("#record-start").on("click", function () {
    data = [{ "time": 0, "temperature": 0 }];
    startAt = Date.now();
    startRun = true;
    $("#record-start").hide();
    $("#record-stop").show();
    $("#time").show();
});

$("#record-stop").on("click", function () {
    startRun = false;
    $("#record-stop").hide();
    $("#record-start").show();
});

$("#shutdown").on('click', function () {
    $.get('api/System/Shutdown');
});

window.setInterval(function () {
    $.get('api/Temperature/GetTemperature', function (data) {
        // If the current temperature is more than 100 degrees different, and it isn't -999 (right after initialization), then don't add it because it must be a bad temperature reading
        if (Math.abs(currentTemp - data) > 100 && currentTemp != -999) {
            currentTemp = currentTemp;
        }
        else
        {
            currentTemp = data;
        }
    });

    UpdateTemperatureDisplay();

    if ($("#hold-state").is(":checked")) {

        if (currentTemp < holdTemp) {
            $.get('api/Heater/ChangeState/On');
        } else {
            $.get('api/Heater/ChangeState/Off');
        }
    }

    if (startRun) {
        UpdateTimeTemperatureData();
        UpdateGraph();
        UpdateTimeDisplay();
    }
}, 100);


function UpdateTemperatureDisplay() {
    $("#numeric-temperature").text(currentTemp);
};
function UpdateTimeDisplay() {
    var elapsedSeconds = (new Date().getTime() - startAt) / 1000;
    var date = new Date(null);
    date.setSeconds(elapsedSeconds); // specify value for SECONDS here
    
    $("#numeric-time").text(date.toISOString().substr(11, 8));
};

function UpdateTimeTemperatureData() {
    var elapsedSeconds = Math.floor((new Date().getTime() - startAt) / 1000);
    data.push({ time: elapsedSeconds, temperature: currentTemp });
};





//Chart
var margin = { top: 20, right: 40, bottom: 30, left: 25 },
    width = document.body.scrollWidth - margin.left - margin.right,
    height = 205 - margin.top - margin.bottom;

var xScale = d3.scaleLinear()
    .domain([0, d3.max(data, function (d, i) {
        if (d.time > 0)
            return d.time - 1;
        else
            return 0;
    })])
    .range([width, width]);

var yScale = d3.scaleLinear()
    .domain([0, 500])
    .range([height, 0]);

var xAxis = d3.axisBottom(xScale)
    .ticks(4, "d");


var yAxis = d3.axisLeft(yScale);

var line = d3.line()
    .x(function (d) { return xScale(d.time); })
    .y(function (d) { return yScale(d.temperature); });

var svg = d3.select("svg")
    .attr("width", width + margin.left + margin.right)
    .attr("height", height + margin.top + margin.bottom)
    .append("g")
    .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

svg.append("defs").append("clipPath")
    .attr("id", "clip")
    .append("rect")
    .attr("width", width)
    .attr("height", height)

svg.select("defs").append("clipPath")
    .attr("id", "clipAxis")
    .append("rect")
    .attr("width", width + margin.left + 5) // +5 so we keep the furthest right x axis label
    .attr("height", (margin.bottom + margin.top))
    .attr("transform", "translate(-" + margin.left + "," + (height - margin.bottom) + ")"); //-margin.left to get the full 0 at the intersection of the x and y axis

svg.append("g").attr("clip-path", "url(#clipAxis)")
    .append("g")
    .attr("class", "x axis")
    .attr("transform", "translate(0," + height + ")")
    .call(xAxis);

svg.append("g")
    .attr("class", "y axis")
    .call(yAxis);

svg.append("g")
    .attr("clip-path", "url(#clip)")
    .attr("width", width)
    .attr("height", height)
    .append("path")
    .datum(data)
    .attr("class", "line")
    .attr("d", line);

svg.append("text")
    .attr("transform", "rotate(-90)")
    .attr("y", 6)
    .attr("dy", ".71em")
    .style("text-anchor", "end")
    .text("Temperature (°F)");

svg.append("text")
    .attr("x", width)
    .attr("y", height-12)
    .attr("dy", ".71em")
    .style("text-anchor", "end")
    .text("Time (sec)")

function UpdateGraph() {
    var elapsedTime = d3.max(data, function (d) { return d.time; });

    if (elapsedTime > 0) {
        xScale.domain([0, d3.max(data, function (d) { return d.time })])
        .range([0, width]);
    }

    // We dynamically change the number of ticks for low time values so we only get round tick values without duplicates
    var numberOfTicks = 4;
    if (elapsedTime < 2) {
        numberOfTicks = 1;
    } else if (elapsedTime < 3) {
        numberOfTicks = 2;
    }

    // Select the section we want to apply our changes to
    var svg = d3.select("svg").transition();

    // Make the changes
    svg.select(".line")   // change the line
      .duration(1000)
      .attr("d", line(data));
    svg.select(".x.axis") // change the x axis
      .duration(1000)
      .call(xAxis.ticks(numberOfTicks));
};


$(function () {
    $(".knob").knob({
        'change': function (v) {
            holdTemp = v;
        }
    });
});