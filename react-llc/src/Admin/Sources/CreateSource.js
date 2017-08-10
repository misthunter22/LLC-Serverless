import React, { Component }           from 'react';
import { LocalForm, Control, Errors } from 'react-redux-form';

class CreateSource extends Component {
  
  handleChange(values) {
  }
  
  handleUpdate(form) {
  }
  
  handleSubmit(values) {
    const formData = {
	  Name:         this.Name.value,
	  Description:  this.Description.value,
	  Checking:     this.AllowLinkChecking.value,
	  Extracting:   this.AllowLinkExtractions.value,
	  ObjectName:   this.Object_Name.value,
	  AccessKey:    this.Object_AccessKey.value,
	  SecretKey:    this.Object_SecretKey.value,
	  Region:       this.Object_Region.value,
	  SearchPrefix: this.Object_SearchPrefix.value
	};
	
    console.log('-->', formData);
  }
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
		  <LocalForm
            onUpdate={(form) => this.handleUpdate(form)}
            onChange={(values) => this.handleChange(values)}
            onSubmit={(values) => this.handleSubmit(values)}
          >
            <h4 style={{"color": "#0ce3ac"}}>Source</h4><hr />
            <div className="form-horizontal">
	          <div className="form-group">
		        <label className="control-label col-md-2" htmlFor="Name">Name</label>
		        <div className="col-md-10">
				  <Control.text getRef={(node) => this.Name = node} model="Name" id="Name" className="form-control text-box single-line"
                    validators={{ required: val => val && val.length }}
				  />
				  <Errors className="errors" model="Name"
                    show={{ touched: true, focus: false }}
                    messages={{ required: 'Name is required' }}
                  />
		        </div>
	          </div>

	          <div className="form-group">
		        <label className="control-label col-md-2" htmlFor="Description">Description</label>
		        <div className="col-md-10">
			      <Control.text getRef={(node) => this.Description = node} model="Description" id="Description" className="form-control text-box single-line"
                    validators={{ required: val => val && val.length }}
				  />
				  <Errors className="errors" model="Description"
                    show={{ touched: true, focus: false }}
                    messages={{ required: 'Description is required' }}
                  />
		        </div>
	          </div>

	          <div className="form-group">
		        <label className="control-label col-md-2" htmlFor="AllowLinkChecking">Allow Link Checking</label>
		        <div className="col-md-10">
		          <input id="AllowLinkChecking" name="AllowLinkChecking" type="checkbox" ref={(node) => this.AllowLinkChecking = node} />
		        </div>
	          </div>

	          <div className="form-group">
		        <label className="control-label col-md-2" htmlFor="AllowLinkExtractions">Allow Link Extractions</label>
		        <div className="col-md-10">
			      <input id="AllowLinkExtractions" name="AllowLinkExtractions" type="checkbox" ref={(node) => this.AllowLinkExtractions = node} />
		        </div>
	          </div>

	          <h4 style={{"marginTop": "20px", "color": "#0ce3ac"}}>Object Source</h4><hr />
	          <div className="form-group">
		        <label className="control-label col-md-2" htmlFor="Object_Name">Name</label>
		        <div className="col-md-10">
				  <Control.text getRef={(node) => this.Object_Name = node} model="Object_Name" id="Object_Name" className="form-control text-box single-line"
                    validators={{ required: val => val && val.length }}
				  />
				  <Errors className="errors" model="Object_Name"
                    show={{ touched: true, focus: false }}
                    messages={{ required: 'S3 object name is required' }}
                  />
		        </div>
	          </div>

	          <div className="form-group">
		        <label className="control-label col-md-2" htmlFor="Object_AccessKey">Access Key</label>
		        <div className="col-md-10">
				  <Control.text getRef={(node) => this.Object_AccessKey = node} model="Object_AccessKey" id="Object_AccessKey" className="form-control text-box single-line"
                    validators={{ required: val => val && val.length }}
				  />
				  <Errors className="errors" model="Object_AccessKey"
                    show={{ touched: true, focus: false }}
                    messages={{ required: 'AWS access key is required' }}
                  />
		        </div>
	          </div>

	          <div className="form-group">
		        <label className="control-label col-md-2" htmlFor="Object_SecretKey">Secret Key</label>
		        <div className="col-md-10">
				  <Control.text getRef={(node) => this.Object_SecretKey = node} model="Object_SecretKey" id="Object_SecretKey" className="form-control text-box single-line"
                    validators={{ required: val => val && val.length }}
				  />
				  <Errors className="errors" model="Object_SecretKey"
                    show={{ touched: true, focus: false }}
                    messages={{ required: 'AWS secret key is required' }}
                  />
		        </div>
	          </div>

	          <div className="form-group">
		        <label className="control-label col-md-2" htmlFor="Object_Region">Region</label>
		        <div className="col-md-10">
				  <Control.text getRef={(node) => this.Object_Region = node} model="Object_Region" id="Object_Region" className="form-control text-box single-line"
                    validators={{ required: val => val && val.length }}
				  />
				  <Errors className="errors" model="Object_Region"
                    show={{ touched: true, focus: false }}
                    messages={{ required: 'AWS region is required' }}
                  />
		        </div>
	          </div>

	          <div className="form-group">
		        <label className="control-label col-md-2" htmlFor="Object_SearchPrefix">Search Prefix</label>
		        <div className="col-md-10">
				  <Control.text getRef={(node) => this.Object_SearchPrefix = node} model="Object_SearchPrefix" id="Object_SearchPrefix" className="form-control text-box single-line" />
		        </div>
	          </div>

	          <div className="form-group">
		        <div className="col-md-offset-2 col-md-10">
			      <input type="submit" value="Save" className="btn btn-default" />
		        </div>
	          </div>
            </div>
          </LocalForm>
		)}
        
      </div>
    );
  }
}

export default CreateSource;
