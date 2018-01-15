import React, { Component } from 'react';
import servicesBase         from '../../Services/ServicesBase';

class CreateSource extends Component {
	
  constructor(props) {
    super(props);
	this.state = {
      source      : null,
	  checking    : false,
	  extractions : false,
	  name        : "",
	  description : "",
	  s3name      : "",
	  access      : "",
	  secret      : "",
	  region      : "",
	  prefix      : "",
	  id          : "",
	  bucket      : ""
	}
	
	this.updateName        = this.updateName.bind(this);
	this.updateDescription = this.updateDescription.bind(this);
	this.updateChecking    = this.updateChecking.bind(this);
	this.updateExtractions = this.updateExtractions.bind(this);
	this.updateS3Name      = this.updateS3Name.bind(this);
	this.updateAccess      = this.updateAccess.bind(this);
	this.updateSecret      = this.updateSecret.bind(this);
	this.updateRegion      = this.updateRegion.bind(this);
	this.updatePrefix      = this.updatePrefix.bind(this);
	this.handleSubmit      = this.handleSubmit.bind(this);
  }
  
  componentDidMount() {
	if (this.props.match.params.id) {
	  var that = this;
	  this.source(this.props.match.params.id)
	    .then(function(source) {
	      that.setState({name:        source.title});
	      that.setState({description: source.description});
	      that.setState({checking:    source.allowlink});
	      that.setState({extractions: source.allowextract});
	      that.setState({s3name:      source.s3name});
	      that.setState({access:      source.access});
	      that.setState({secret:      source.secret});
	      that.setState({region:      source.region});
	      that.setState({prefix:      source.prefix});
	      that.setState({id:          source.source});
	      that.setState({bucket:      source.bucket});
	  });
	}
  }
  
  updateName(event) {
    this.setState({name: event.target.value})
  }
  
  updateDescription(event) {
    this.setState({description: event.target.value})
  }
  
  updateChecking(event) {
	this.setState({checking: event.target.checked});
  }
  
  updateExtractions(event) {
	this.setState({extractions: event.target.checked});
  }
  
  updateS3Name(event) {
    this.setState({s3name: event.target.value})
  }
  
  updateAccess(event) {
    this.setState({access: event.target.value})
  }
  
  updateSecret(event) {
    this.setState({secret: event.target.value})
  }
  
  updateRegion(event) {
    this.setState({region: event.target.value})
  }
  
  updatePrefix(event) {
    this.setState({prefix: event.target.value})
  }
  
  handleSubmit(e) {
	e.preventDefault();
	
    const formData = {
	  Id:                   this.state.id,
	  Name:                 this.state.name,
	  Description:          this.state.description,
	  AllowLinkChecking:    this.state.checking,
	  AllowLinkExtractions: this.state.extractions,
	  S3BucketId:           this.state.bucket,
	  S3bucketSearchPrefix: this.state.prefix,
	  S3BucketName:         this.state.s3name,
	  Region:               this.state.region,
	  AccessKey:            this.state.access,
	  SecretKey:            this.state.secret
	};
	
	this.changeSource(formData)
	  .then(function(result) {
		if (result.status === true) {
	      window.location = "/admin/sources"
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
		  <form onSubmit={this.handleSubmit}>
            <h4 style={{"color": "#0ce3ac"}}>Source</h4><hr />
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
				  <input type="text" value={this.state.description} onChange={this.updateDescription} className="form-control text-box single-line" />
		        </div>
	          </div>

	          <div className="form-group">
		        <label className="control-label col-md-2">Allow Link Checking</label>
		        <div className="col-md-10">
				  <input type="checkbox" checked={this.state.checking} onChange={this.updateChecking} />
		        </div>
	          </div>

	          <div className="form-group">
		        <label className="control-label col-md-2">Allow Link Extractions</label>
		        <div className="col-md-10">
				  <input type="checkbox" checked={this.state.extractions} onChange={this.updateExtractions} />
		        </div>
	          </div>

	          <h4 style={{"marginTop": "20px", "color": "#0ce3ac"}}>Object Source</h4><hr />
	          <div className="form-group">
		        <label className="control-label col-md-2">Name</label>
		        <div className="col-md-10">
				  <input type="text" value={this.state.s3name} onChange={this.updateS3Name} className="form-control text-box single-line" />
		        </div>
	          </div>

	          {/* <div className="form-group">
		        <label className="control-label col-md-2">Access Key</label>
		        <div className="col-md-10">
				  <input type="text" value={this.state.access} onChange={this.updateAccess} className="form-control text-box single-line" />
		        </div>
	          </div>

	          <div className="form-group">
		        <label className="control-label col-md-2">Secret Key</label>
		        <div className="col-md-10">
				  <input type="text" value={this.state.secret} onChange={this.updateSecret} className="form-control text-box single-line" />
		        </div>
	          </div> */}

	          <div className="form-group">
		        <label className="control-label col-md-2">Region</label>
		        <div className="col-md-10">
				  <input type="text" value={this.state.region} onChange={this.updateRegion} className="form-control text-box single-line" />
		        </div>
	          </div>

	          <div className="form-group">
		        <label className="control-label col-md-2">Search Prefix</label>
		        <div className="col-md-10">
				  <input type="text" value={this.state.prefix} onChange={this.updatePrefix} className="form-control text-box single-line" />
		        </div>
	          </div>

	          <div className="form-group">
		        <div className="col-md-offset-2 col-md-10">
			      <input type="submit" value="Save" className="btn btn-default" />
		        </div>
	          </div>
            </div>
          </form>
		)}
        
      </div>
    );
  }
}

export default servicesBase(CreateSource);
