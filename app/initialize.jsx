/* jshint node: true */
"use strict";

require("stores");

var React = require("react"),
    Llc = require("pages/llc");

$(function () {
    React.render(new Llc(), $("#app")[0]);
});