var AWS    = require('aws-sdk');
var Client = require('aws-api-gateway-client').default;

export const AwsConstants = {
    region: 'us-west-2',
    invokeUrl: 'https://fkpj4ljuxh.execute-api.us-west-2.amazonaws.com',
    identityPoolId: 'us-west-2:b4c04701-3f71-4618-8a18-c9a72742dc7b'
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
	
	applySource(that, obj) {
	  that.setState((prevState, props) => ({
	    sources: prevState.sources.concat([obj])
	  }));
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
		
		var pathTemplate     = '/Prod/api/sources';
		var params           = {};
		var additionalParams = {};
		var body             = {};
		var method           = 'GET';
		
		apigClient.invokeApi(params, pathTemplate, method, additionalParams, body)
		  .then(function(result) {
		  	for (var i = 0; i < result.data.result.length; i++) {
			  var s = result.data.result[i];
			  if (includeZero || s.source > 0) {
				var obj  = {};
				obj['source']    = s.source;
				obj['title']     = s.title;
				obj['objects']   = s.s3ObjectCount;
				obj['html']      = s.htmlFileCount;
				obj['links']     = s.linkCount;
				obj['extracted'] = s.dateLastExtracted;
				obj['invalid']   = s.invalidLinkCount;
				obj['checked']   = s.dateLastChecked;
				  
				that.applySource(that, obj);
			  }
			}
		  }).catch(function(result){
		  console.log(result);
	    });
	  });
	}
  }

  return ServicesBase;
}
