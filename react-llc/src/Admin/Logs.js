import React, { Component } from 'react';

class Logs extends Component {
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
		    <div>
              <h3>Logs</h3>
			  <span>Logs are stored under <a href="https://us-west-2.console.aws.amazon.com/cloudwatch/home?region=us-west-2#logs:" target="_blank">CloudWatch</a> for the account the application is
			        running under. The best place to start looking is in the particular
					<a href="https://us-west-2.console.aws.amazon.com/lambda/home?region=us-west-2#/functions" target="_blank"> lambda function </a> 
					that is failing. Look for this page to be updated with convenience
					operations in the future.
			  </span>
			</div>
            )
        }
        
      </div>
    );
  }
}

export default Logs;
