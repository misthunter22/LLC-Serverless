var AWS = require('aws-sdk');

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
    }

    return ServicesBase;
}
