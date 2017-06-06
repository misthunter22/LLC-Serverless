import React, { Component } from 'react';
import NoAuth from '../Auth/NoAuth';

class Home extends Component {
  login() {
    this.props.auth.login();
  }
  render() {
	const { isAuthenticated } = this.props.auth;
    return (
	  <div className="container">
      {
        isAuthenticated() && (
	      <div className="jumbotron">
	        <h1>Lor Link Checker</h1>
            <p className="lead">An Idaho Digital Learning Web Application.</p>
            <p>
		      <a href="http://idahodigitallearning.org" target="_blank" className="btn btn-primary btn-lg">
			    Learn more about IDLA »
			  </a>
		    </p>
          </div>
		)
	  }
	  {
        !isAuthenticated() && (
          <NoAuth />
        )
      }
      </div>
    );
  }
}

export default Home;
