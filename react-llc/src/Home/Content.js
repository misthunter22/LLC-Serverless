import React, { Component } from 'react';

class Content extends Component {
  render() {
    return (
    <div>
		<div className="jumbotron">
	   		<h1>Lor Link Checker</h1>
	   		<p className="lead">An Idaho Digital Learning Web Application.</p>
	   		<p>
		  		<a href="http://idahodigitallearning.org" target="_blank" rel="noopener noreferrer" className="btn btn-primary btn-lg">
		  			Learn more about IDLA »
		  		</a>
	   		</p>
		</div>
		<div className="row">
		  <div className="col-md-4">
			 <h2>Reports</h2>
			 <div className="height-50">
				View and/or export reports on monitored links.
			 </div>
			 <p><a className="btn btn-info" href="/#/report">View &raquo;</a>
			 </p>
		  </div>
		  <div className="col-md-4">
			 <h2>Upload Links</h2>
			 <div className="height-70">
				Upload Blackboard and Brainhoney course packages to extract all Urls for future monitoring.
			 </div>
			 <p><a className="btn btn-info" href="/#/admin/upload">Upload &raquo;</a>
			 </p>
		  </div>
		  <div className="col-md-4">
			 <h2>Manage Site</h2>
			 <div className="height-50">
				Manage LLC settings and users. View Logs.
			 </div>
			 <p><a className="btn btn-info" href="/#/admin">Admin &raquo;</a>
			 </p>
		  </div>
	   </div>
	</div>);
  }
}

export default Content;
