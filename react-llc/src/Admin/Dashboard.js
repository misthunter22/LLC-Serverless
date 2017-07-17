import React, { Component } from 'react';
import { Button } from 'react-bootstrap';
import servicesBase, {AwsConstants} from '../Services/ServicesBase';

var AWS    = require('aws-sdk');
var Client = require('aws-api-gateway-client').default;

class Dashboard extends Component {

    constructor(props) {
        super(props);
        this.state = {
            object: '',
            links: '',
            invalid: '',
            html: '',
            extracted: '',
            checked: ''
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
        return <div>
            <h2 className="bottom-20">Administrative Dashboard</h2>
            <div className="col-md-5 bottom-margin-10">
                <Button
                    className="btn btn-lg btn-default btn-mt"
                    //onClick={this.goTo.bind(this, 'sources')}
                    >
                    Sources
                </Button>
                <Button
                    className="btn btn-lg btn-default btn-mt"
                    //onClick={this.goTo.bind(this, 'settings')}
                    >
                    Settings
                </Button>
                <Button
                    className="btn btn-lg btn-default btn-mt"
                    //onClick={this.goTo.bind(this, 'logs')}
                    >
                    Logs
                </Button>
                <Button
                    className="btn btn-lg btn-default btn-mt"
                    //onClick={this.goTo.bind(this, 'users')}
                    >
                    Users
                </Button>
                <Button
                    className="btn btn-lg btn-default btn-mt"
                    //onClick={this.goTo.bind(this, 'roles')}
                    >
                    Roles
                </Button>
            </div>
        </div>;
    }
}

export default servicesBase(Dashboard);
