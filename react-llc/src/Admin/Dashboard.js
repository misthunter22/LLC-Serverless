import React, { Component } from 'react';
import axios from 'axios';

var AWS    = require('aws-sdk');
var Client = require('aws-api-gateway-client').default;

class Dashboard extends Component {
  
  render() {
	  
	// Initialize the Amazon Cognito credentials provider
	AWS.config.region = 'us-west-2'; // Region
	AWS.config.credentials = new AWS.CognitoIdentityCredentials({
	  IdentityPoolId: 'us-west-2:b4c04701-3f71-4618-8a18-c9a72742dc7b',
	  Logins: {
        'idla-auth.auth0.com': localStorage.getItem('id_token')
      }
	});
	
	AWS.config.credentials.get(function(){

      // Credentials will be available when this function is called.
      var accessKeyId = AWS.config.credentials.accessKeyId;
      var secretAccessKey = AWS.config.credentials.secretAccessKey;
      var sessionToken = AWS.config.credentials.sessionToken;
	  
	  var apigClient = Client.newClient({
		invokeUrl: 'https://fkpj4ljuxh.execute-api.us-west-2.amazonaws.com',
	    accessKey: accessKeyId,
        secretKey: secretAccessKey,	
        sessionToken: sessionToken,
        region: 'us-west-2'
      });

      var params = {
      };
	  
      var pathTemplate = '/Prod/api/admin'
      var method = 'GET';
      var additionalParams = {
      };
	  
      var body = {
      };

      apigClient.invokeApi(params, pathTemplate, method, additionalParams, body)
        .then(function(result){
          console.log(result);
        }).catch( function(result){
          console.log(result);
      });
    });
	
	return <div></div>;
  }
}

export default Dashboard;
