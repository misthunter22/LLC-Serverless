import React            from 'react';
import { PacmanLoader } from 'react-spinners';

var AWS    = require('aws-sdk');
var Client = require('aws-api-gateway-client').default;
const $    = require('jquery');

export const AwsConstants = {
    region: 'us-west-2',
    invokeUrl: 'https://j6nbqh9lm5.execute-api.us-west-2.amazonaws.com',
    identityPoolId: 'us-west-2:b4c04701-3f71-4618-8a18-c9a72742dc7b',
	environment: '/Prod'
};

export default function servicesBase(Component) {

  class ServicesBase extends Component {

    constructor(props) {
      super(props);

      // Initialize the Amazon Cognito credentials provider
      AWS.config.region = AwsConstants.region; // Region
      AWS.config.credentials = new AWS.CognitoIdentityCredentials({
        IdentityPoolId: AwsConstants.identityPoolId,
        Logins: {
          'idla-auth.auth0.com': localStorage.getItem('id_token')
        }
      });
    }
	
	spinnerMarkup() {
	  let spinner = 
	    <div id="loading_spinner" style={{'marginLeft': '50%'}}>
	      <PacmanLoader
	        color={'#0ce3ac'} 
		  />
	    </div>;
		
	  return spinner;
	}
	
	changeSpinner(that, state) {
	  that.setState({loading: state});
	  if (state)
		$('#loading_spinner').show();
	  else
		$('#loading_spinner').hide();
	}
	
	applySource(that, a) {
	  var array = that.state.sources; 
	  for (var i = 0; i < a.length; i++) {
	    array.push(a[i]);
	  }
      that.setState({sources:array})
	}
	
	applySetting(that, a) {
	  var array = that.state.settings; 
	  for (var i = 0; i < a.length; i++) {
	    array.push(a[i]);
	  }
      that.setState({settings:array})
	}
	
	applyInvalidLinks(that, a) {
      that.setState({invalidLinks:a})
	}
	
	applyWarningLinks(that, a) {
      that.setState({warningLinks:a})
	}
		
	sources(includeZero = false) {
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
		
		var pathTemplate     = AwsConstants.environment + '/api/sources';
		var params           = {};
		var additionalParams = {};
		var body             = {};
		var method           = 'GET';
		
		apigClient.invokeApi(params, pathTemplate, method, additionalParams, body)
		  .then(function(result) {
			var push = [];
		  	for (var i = 0; i < result.data.length; i++) {
			  var s = result.data[i];
			  if (includeZero || s.id > 0) {
				var obj  = {};
				obj['source']       = s.id;
				obj['title']        = s.name;
				obj['objects']      = s.s3ObjectCount;
				obj['html']         = s.htmlFileCount;
				obj['links']        = s.linkCount;
				obj['extracted']    = s.dateLastExtracted;
				obj['invalid']      = s.invalidLinkCount;
				obj['checked']      = s.dateLastChecked;
				obj['s3name']       = s.s3bucketName;
				obj['allowlink']    = s.allowLinkChecking;
				obj['allowextract'] = s.allowLinkExtractions; 
				obj['created']      = s.dateCreated;
				  
				push.push(obj);
			  }
			}
			
			that.applySource(that, push);
		  }).catch(function(result){
		  console.log(result);
	    });
	  });
	}
	
	settings() {
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
		
		var pathTemplate     = AwsConstants.environment + '/api/settings';
		var params           = {};
		var additionalParams = {};
		var body             = {};
		var method           = 'GET';
		
		apigClient.invokeApi(params, pathTemplate, method, additionalParams, body)
		  .then(function(result) {
			var push = [];
		  	for (var i = 0; i < result.data.length; i++) {
			  var s = result.data[i];
			  var obj  = {};
			  obj['id']          = s.id;
			  obj['created']     = s.dateCreated;
			  obj['modified']    = s.dateModified;
			  obj['description'] = s.description;
			  obj['user']        = s.modifiedUser;
			  obj['name']        = s.name;
			  obj['value']       = s.value;
				  
			  push.push(obj);
			}
			
			that.applySetting(that, push);
		  }).catch(function(result){
		  console.log(result);
	    });
	  });
	}
	
	invalidLinks(settings) {
	  var that = this;
	  return new Promise(function (fulfill, reject) {
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
		
		  var pathTemplate     = AwsConstants.environment + '/api/invalidReport';
		  var body             = {
			  draw:       settings.draw, 
			  start:      settings.start, 
			  length:     settings.length,
			  column:     settings.order[0].column,
			  columnName: settings.columns[settings.order[0].column].data,
			  direction:  settings.order[0].dir,
			  search:     settings.search.value
	      };
		  var additionalParams = {};
		  var params           = {};
		  var method           = 'POST';
		
		  apigClient.invokeApi(params, pathTemplate, method, additionalParams, body)
		    .then(function(result) {
			  that.applyInvalidLinks(that, result.data);
			  fulfill(result);
		    }).catch(function(result){
		    console.log(result);
			reject(result);
	      });
	    });
	  });
	}
	
	warningLinks(settings) {
	  var that = this;
	  return new Promise(function (fulfill, reject) {
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
		
		  var pathTemplate     = AwsConstants.environment + '/api/warningReport';
		  var body             = {
			  draw:       settings.draw, 
			  start:      settings.start, 
			  length:     settings.length,
			  column:     settings.order[0].column,
			  columnName: settings.columns[settings.order[0].column].data,
			  direction:  settings.order[0].dir,
			  search:     settings.search.value
	      };
		  var additionalParams = {};
		  var params           = {};
		  var method           = 'POST';
		
		  apigClient.invokeApi(params, pathTemplate, method, additionalParams, body)
		    .then(function(result) {
			  that.applyWarningLinks(that, result.data);
			  fulfill(result);
		    }).catch(function(result){
		    console.log(result);
			reject(result);
	      });
	    });
	  });
	}
	
    bucketLocations(data, type) {
	  return new Promise(function (fulfill, reject) {
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
		
		  var pathTemplate     = AwsConstants.environment + '/api/bucketLocations';
		  var body             = {
			  id: data,
			  type: type
	      };
		  var additionalParams = {};
		  var params           = {};
		  var method           = 'POST';
		
		  apigClient.invokeApi(params, pathTemplate, method, additionalParams, body)
		    .then(function(result) {
			  fulfill(result);
		    }).catch(function(result){
		    console.log(result);
			reject(result);
	      });
	    });
	  });
	}
	
	configureScrenshotLinks() {
		
	  var that = this;

	  // Match to Bootstraps data-toggle for the modal
	  // and attach an onclick event handler
	  $('a.btn-info,a.label').off().on('click', function (e) {

		if ($(this).hasClass("selected") || $(this).closest("tr").hasClass("selected")) {
			$(this).removeClass("selected");
		}
		else {
			$('tr.selected').removeClass('selected');
			if ($(this).is("a")) {
				$(this).closest("tr").addClass("selected");
			} else {
				$(this).addClass("selected");
			}
		}

		if ($(this).hasClass("bucket-locations")) {
			
			// From the clicked element, get the data-target arrtibute
			// which BS3 uses to determine the target modal
			var target_modal = $(e.currentTarget).data('target');
			// also get the remote content's URL
			var remote_content = $(e.currentTarget).data('src');
			var remote_type    = $(e.currentTarget).data('type');

			// Find the target modal in the DOM
			var $modal = $(target_modal);
			// Find the modal's <div class="modal-body"> so we can populate it
			var $modalBody = $(target_modal + ' .modal-body');

			// Capture BS3's show.bs.modal which is fires
			// immediately when, you guessed it, the show instance method
			// for the modal is called
			$modal.off().on('show.bs.modal', function () {
				$modalBody.empty();

				// use your remote content URL to load the modal body
				that.bucketLocations(remote_content, remote_type).then(function(d) {
				  var str = '<ul>';
				  $.each( d.data, function( key, value ) {
					str += '<li><a href="' + value.data + '" target="_blank">' + value.data + '</a></li>';
                  });
				  str += '</ul>';
				  if (d.data.length === 0) {
				    str = 'No Results';
				  }
				  $modalBody.html(str);
				});
			}).modal();
			// and show the modal

			// Now return a false (negating the link action) to prevent Bootstrap's JS 3.1.1
			// from throwing a 'preventDefault' error due to us overriding the anchor usage.
			return false;
		}

		return true;
	  });
    }
  }

  return ServicesBase;
}
