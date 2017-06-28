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
	   <div className="row">
		  <div className="col-md-4">
			 <h2>Reports</h2>
			 <div className="height-50">
				View and/or export reports on monitored links.
			 </div>
			 <p><a className="btn btn-info" href="Report">View &raquo;</a>
			 </p>
		  </div>
		  <div className="col-md-4">
			 <h2>Upload Links</h2>
			 <div className="height-70">
				Upload Blackboard and Brainhoney course packages to extract all Urls for future monitoring.
			 </div>
			 <p><a className="btn btn-info" href="Upload">Upload &raquo;</a>
			 </p>
		  </div>
		  <div className="col-md-4">
			 <h2>Manage Site</h2>
			 <div className="height-50">
				Manage LLC settings and users. View Logs.
			 </div>
			 <p><a className="btn btn-info" href="Admin">Admin &raquo;</a>
			 </p>
		  </div>
	   </div>
	</div>
    );
  }
}

export default Content;
