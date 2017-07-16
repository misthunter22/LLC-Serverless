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

            var pathTemplates = [
                'object',
                'links',
                'invalid',
                'html',
                'extracted',
                'checked'
            ];

            pathTemplates.forEach(function(element) {
                var method = 'GET';
                var pathTemplate = '/Prod/api/admin/' + element;

                var params = {};
                var additionalParams = {};
                var body = {};

                apigClient.invokeApi(params, pathTemplate, method, additionalParams, body)
                    .then(function(result){
                        var obj = {};
                        obj[element] = result.data.data;
                        that.setState(obj);
                    }).catch(function(result){
                    console.log(result);
                });
            });
        });
    }
  
    render() {
        return <div>{this.state.object},{this.state.links},{this.state.invalid},{this.state.html}</div>;
    }
}

export default servicesBase(Dashboard);
