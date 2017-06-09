import React, { Component } from 'react';
import axios from 'axios';

var AWS = require('aws-sdk');

class Profile extends Component {
  data() {
	AWS.config.credentials.get(function() {
	  
	})
  }
}

export default Profile;
