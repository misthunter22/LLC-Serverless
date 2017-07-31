import React, { Component } from 'react';
import { Button } from 'react-bootstrap';
import servicesBase, {AwsConstants} from '../Services/ServicesBase';

var AWS    = require('aws-sdk');
var Client = require('aws-api-gateway-client').default;

class Dashboard extends Component {

    constructor(props) {
        super(props);
		this.state = {
		  sources: []
		}
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
			
			var pathTemplate     = '/Prod/api/sources';
			var params           = {};
			var additionalParams = {};
			var body             = {};
			var method           = 'GET';
			
			apigClient.invokeApi(params, pathTemplate, method, additionalParams, body)
				.then(function(result) {
					for (var i = 0; i < result.data.result.length; i++) {
					  var s    = result.data.result[i];
					  var path = '/Prod/api/dashboard/' + s.source;
					  apigClient.invokeApi(params, path, method, additionalParams, body)
						.then(function(result) {
						  var obj  = {};
						  var data = result.data;
						  obj['source'] = data.source;
						  obj['title']  = s.title;
						  for (var j = 0; j < data.data.length; j++) {
							var dj      = data.data[j];
					        obj[dj.key] = dj.data;
						  }
						  
						  that.setState((prevState, props) => ({
						    sources: prevState.sources.concat([obj])
						  }));
						});
					}
				}).catch(function(result){
				console.log(result);
			});
        });
    }
  
    render() {
	  let sources = [];
	  if (this.state.sources.length > 0) {
	    for (var i = 0; i < this.state.sources.length; i++) {
		  var source = this.state.sources[i];
		  sources.push(<div key={source.source}>{source.title}</div>);  
	    }
	  }
	  
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
		{sources}
      </div>;
    }
}

export default servicesBase(Dashboard);
