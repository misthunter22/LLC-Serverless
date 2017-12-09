import React, { Component } from 'react';
import servicesBase         from '../../Services/ServicesBase';

class ManageSetting extends Component {
	
  constructor(props) {
    super(props);
	this.state = {
      setting     : null,
	  id          : "",
	  name        : "",
	  value       : "",
	  description : ""
	}
	
	this.updateName        = this.updateName.bind(this);
	this.updateDescription = this.updateDescription.bind(this);
	this.updateValue       = this.updateValue.bind(this);
	this.handleSubmit      = this.handleSubmit.bind(this);
  }
  
  componentDidMount() {
	if (this.props.match.params.id) {
	  var that = this;
	  this.source(this.props.match.params.id)
	    .then(function(source) {
	      that.setState({id:          source.id});
	      that.setState({name:        source.name});
		  that.setState({value:       source.value});
	      that.setState({description: source.description});
	  });
	}
  }
  
  updateName(event) {
    this.setState({name: event.target.value})
  }
  
  updateDescription(event) {
    this.setState({description: event.target.value})
  }
  
  updateValue(event) {
    this.setState({value: event.target.value})
  }
  
  handleSubmit(e) {
	e.preventDefault();
	
    const formData = {
	  Id:          this.state.id,
	  Name:        this.state.name,
	  Value:       this.state.value,
	  Description: this.state.description
	};
	
	this.saveSource(formData)
	  .then(function(result) {
		if (result.status === true) {
	      window.location = "/admin/settings"
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
		        <label className="control-label col-md-2">Value</label>
		        <div className="col-md-10">
				  <input type="text" value={this.state.value} onChange={this.updateValue} className="form-control text-box single-line" />
		        </div>
	          </div>

	          <div className="form-group">
		        <label className="control-label col-md-2">Description</label>
		        <div className="col-md-10">
				  <input type="text" value={this.state.description} onChange={this.updateDescription} className="form-control text-box single-line" />
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

export default servicesBase(ManageSetting);
