import React, { Component } from 'react';
import Profile from '../Services/Profile';

class Content extends Component {
  render() {
    return (
	  <div className="jumbotron">
	    <h1>Lor Link Checker</h1>
        <p className="lead">An Idaho Digital Learning Web Application.</p>
        <p>
		  <a href="http://idahodigitallearning.org" target="_blank" className="btn btn-primary btn-lg">
			Learn more about IDLA »
	      </a>
		</p>
		<Profile />
      </div>
    );
  }
}

export default Content;
