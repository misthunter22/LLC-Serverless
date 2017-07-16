import React, { Component } from 'react';
import servicesBase, {AwsConstants} from '../Services/ServicesBase';

var AWS    = require('aws-sdk');
var Client = require('aws-api-gateway-client').default;

class Dashboard extends Component {

    constructor(props) {
        super(props);
        this.state = {
            object   : '',
            links    : '',
            invalid  : '',
            html     : '',
            extracted: '',
            checked  : ''
        };
    }

    componentDidMount() {
        var that = this;
        AWS.config.credentials.get(function(){

            // Credentials will be available when this function is called.
            var accessKeyId = AWS.config.credentials.accessKeyId;
            var secretAccessKey = AWS.config.credentials.secretAccessKey;
            var sessionToken = AWS.config.credentials.sessionToken;

            var apigClient = Client.newClient({
                invokeUrl: AwsConstants.invokeUrl,
                accessKey: accessKeyId,
                secretKey: secretAccessKey,
                sessionToken: sessionToken,
                region: AwsConstants.region
            });

            var params = {
            };

            var pathTemplate = '/Prod/api/admin/invalid'
            var method = 'GET';
            var additionalParams = {
            };

            var body = {
            };

            apigClient.invokeApi(params, pathTemplate, method, additionalParams, body)
                .then(function(result){
                    that.setState({ invalid: result.data.data });
                }).catch(function(result){
                    console.log(result);
                });
        });
    }
  
    render() {
        return <div>{this.state.invalid}</div>;
    }
}

export default servicesBase(Dashboard);
