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
	
	goTo(route) {
      this.props.history.replace(`/${route}`)
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
		  let s = <div className="col-md-5 label-default dash-div">
            <h3 className="label-default dash-h3-label">{source.title}</h3>

            <div className="label label-default dash-title">Total S3 Objects: </div>
            <div className="label label-primary dash-value">
			  {source.objects}
            </div>
			
            <div className="dash-title-odd"> </div>
			
            <div className="label label-default dash-title">HTML Files: </div>
            <div className="label label-info dash-value">
              {source.html}
            </div>

            <div className="dash-title-even"> </div>

            <div className="label label-default dash-title">Total Links: </div>
            <div className="label label-success dash-value">
              {source.links}
            </div>
			
            <div className="dash-title-odd"> </div>
			
            <div className="label label-default dash-title">Last Extracted: </div>
            <div className="label label-info dash-value"
                 title="Links were last extracted  @source.LinksLastExtracted.ToString()">
              {source.extracted}
            </div>

            <div className="dash-title-even"></div>

            <div className="label label-default dash-title">Invalid Links: </div>
            <div className="label label-danger dash-value">
              {source.invalid}
            </div>
			
            <div className="dash-title-odd"> </div>
			
            <div className="label label-default dash-title">Last Checked: </div>
            <div className="label label-info dash-value"
                 title="Links were last checked  @source.LinksLastChecked.ToString()">
              {source.checked}
            </div>
          </div>;
		  sources.push(s);  
	    }
	  }
	  
      return <div>
        <h2 className="bottom-20">Administrative Dashboard</h2>
        <div className="col-md-5 bottom-margin-10">
		  <Button
			className="btn btn-lg btn-default btn-mt"
			onClick={this.goTo.bind(this, 'admin/sources')}
			>
			Sources
		  </Button>
		  <Button
			className="btn btn-lg btn-default btn-mt"
			onClick={this.goTo.bind(this, 'admin/settings')}
			>
			Settings
	  	  </Button>
		  <Button
			className="btn btn-lg btn-default btn-mt"
			onClick={this.goTo.bind(this, 'admin/logs')}
			>
			Logs
		  </Button>
		  <Button
			className="btn btn-lg btn-default btn-mt"
			onClick={this.goTo.bind(this, 'admin/users')}
			>
			Users
		  </Button>
		  <Button
			className="btn btn-lg btn-default btn-mt"
			onClick={this.goTo.bind(this, 'admin/roles')}
			>
			Roles
		  </Button>
	    </div>
		{sources}
      </div>;
    }
}

export default servicesBase(Dashboard);
