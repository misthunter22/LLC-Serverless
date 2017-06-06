import React, { Component } from 'react';

class Home extends Component {
  login() {
    this.props.auth.login();
  }
  render() {
    return (
      <div className="container">
	    <div className="jumbotron">
	      <h1>Lor Link Checker</h1>
          <p className="lead">An Idaho Digital Learning Web Application.</p>
          <p>
		    <a href="http://idahodigitallearning.org" target="_blank" className="btn btn-primary btn-lg">
			  Learn more about IDLA »
			</a>
		  </p>
        </div>
      </div>
    );
  }
}

export default Home;
