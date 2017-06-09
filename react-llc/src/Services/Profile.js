import React, { Component } from 'react';
var AWS = require('aws-sdk');

class Profile extends Component {
  data() {
	AWS.config.credentials.get(function() {
	  
	})
  }
}

export default Profile;
