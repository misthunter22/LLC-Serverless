import React, { Component } from 'react';
import FileBase64           from 'react-file-base64';
import servicesBase         from '../../Services/ServicesBase';

class CreateUpload extends Component {
	
  constructor(props) {
    super(props);
	this.state = {
	  name         : "",
	  fileName     : "",
	  fileContents : "",
	  description  : ""
	}
	
	this.updateName        = this.updateName.bind(this);
	this.updateDescription = this.updateDescription.bind(this);
	this.updateFile        = this.updateFile.bind(this);
	this.handleSubmit      = this.handleSubmit.bind(this);
  }
  
  updateName(event) {
    this.setState({name: event.target.value})
  }
  
  updateDescription(event) {
    this.setState({description: event.target.value})
  }
  
  updateFile(file) {
	this.setState({ fileName: file.name, fileContents: file.base64 });
  }
  
  handleSubmit(e) {
	e.preventDefault();
	
    const formData = {
	  Name         : this.state.name,
	  FileName     : this.state.fileName,
	  FileContents : this.state.fileContents,
	  Description  : this.state.description
	};
	
	this.createPackage(formData)
	  .then(function(result) {
		if (result.status === true) {
	      window.location = "/#/admin/upload"
		}
	  })
	  .catch(function(result) {
		console.log(JSON.stringify(result));  
	  });
  }
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
		  <form onSubmit={this.handleSubmit} ref="createUpload"  encType="multipart/form-data">
            <h4 style={{"color": "#0ce3ac"}}>Create Upload</h4><hr />
            <div className="form-horizontal">
	          <div className="form-group">
		        <label className="control-label col-md-2">Name</label>
		        <div className="col-md-10">
				  <input type="text" value={this.state.name} onChange={this.updateName} className="form-control text-box single-line" />
		        </div>
	          </div>

	          <div className="form-group">
		        <label className="control-label col-md-2">Description</label>
		        <div className="col-md-10">
				  <textarea cols="50" rows="4" value={this.state.description} onChange={this.updateDescription} className="form-control text-box single-line" />
		        </div>
	          </div>
			  
              <div className="form-group">
		        <label className="control-label col-md-2">File</label>
		        <div className="col-md-10">
				  <FileBase64 multiple={false} onDone={this.updateFile.bind(this)} />
		        </div>
	          </div>

	          <div className="form-group">
		        <div className="col-md-offset-2 col-md-10">
			      <input type="submit" value="Create" className="btn btn-default" />
		        </div>
	          </div>
            </div>
          </form>
		)}
        
      </div>
    );
  }
}

export default servicesBase(CreateUpload);
