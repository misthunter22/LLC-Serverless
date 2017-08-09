import React, { Component } from 'react';

class CreateSource extends Component {
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
		  <div>
            <h4 style={{"color": "#0ce3ac"}}>Source</h4><hr />
            <div className="form-horizontal">
	          <div className="form-group">
		        <label className="control-label col-md-2" for="Name">Name</label>
		        <div className="col-md-10">
			      <input className="form-control text-box single-line" id="Name" name="Name" type="text" value="" />
		        </div>
	          </div>

	        <div className="form-group">
		      <label className="control-label col-md-2" for="Description">Description</label>
		      <div className="col-md-10">
			    <input className="form-control text-box single-line" id="Description" name="Description" type="text" value="" />
		      </div>
	        </div>

	        <div className="form-group">
		      <label className="control-label col-md-2" for="AllowLinkChecking">Allow Link Checking</label>
		      <div className="col-md-10">
		        <input id="AllowLinkChecking" name="AllowLinkChecking" type="checkbox" value="true" />
		      </div>
	        </div>

	        <div className="form-group">
		      <label className="control-label col-md-2" for="AllowLinkExtractions">Allow Link Extractions</label>
		      <div className="col-md-10">
			    <input id="AllowLinkExtractions" name="AllowLinkExtractions" type="checkbox" value="true" />
		      </div>
	        </div>

	        <h4 style={{"margin-top": "20px", "color": "#0ce3ac"}}>Object Source</h4><hr />
	        <div className="form-group">
		      <label className="control-label col-md-2" for="Object_Name">Name</label>
		      <div className="col-md-10">
			    <input className="form-control text-box single-line" id="Object_Name" name="Object.Name" type="text" value="" />
		      </div>
	        </div>

	        <div className="form-group">
		      <label className="control-label col-md-2" for="Object_AccessKey">Access Key</label>
		      <div className="col-md-10">
			    <input className="form-control text-box single-line" id="Object_AccessKey" name="Object.AccessKey" type="text" value="" />
		      </div>
	        </div>

	        <div className="form-group">
		      <label className="control-label col-md-2" for="Object_SecretKey">Secret Key</label>
		      <div className="col-md-10">
			    <input className="form-control text-box single-line" id="Object_SecretKey" name="Object.SecretKey" type="text" value="" />
		      </div>
	        </div>

	        <div className="form-group">
		      <label className="control-label col-md-2" for="Object_Region">Region</label>
		      <div className="col-md-10">
			    <input className="form-control text-box single-line" id="Object_Region" name="Object.Region" type="text" value="" />
		      </div>
	        </div>

	        <div className="form-group">
		      <label className="control-label col-md-2" for="Object_SearchPrefix">Search Prefix</label>
		      <div className="col-md-10">
			    <input className="form-control text-box single-line" id="Object_SearchPrefix" name="Object.SearchPrefix" type="text" value="" />
		      </div>
	        </div>

	        <div className="form-group">
		      <div className="col-md-offset-2 col-md-10">
			    <input type="submit" value="Save" className="btn btn-default" />
		      </div>
	        </div>
          </div>
        </div>
		)}
        
      </div>
    );
  }
}

export default CreateSource;
