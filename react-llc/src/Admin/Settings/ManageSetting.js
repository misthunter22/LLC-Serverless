import React, { Component } from 'react';
import servicesBase         from '../../Services/ServicesBase';

class ManageSetting extends Component {
	
  constructor(props) {
    super(props);
	this.state = {
	  nameValid   : true,
	  valueValid  : true,
	  descrValid  : true,
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
	  this.setting(this.props.match.params.id)
	    .then(function(source) {
	      that.setState({id:          source.id});
	      that.setState({name:        source.name});
		  that.setState({value:       source.value});
	      that.setState({description: source.description});
	  });
	}
  }
  
  updateName(event) {
    this.setState({name: event.target.value}, function() {
	  this.isValid();
	});
  }
  
  updateDescription(event) {
    this.setState({description: event.target.value}, function() {
	  this.isValid();
	});
  }
  
  updateValue(event) {
    this.setState({value: event.target.value}, function() {
	  this.isValid();
	});
  }
  
  isValid() {
	if (!this.state.name) {
	  this.setState({nameValid: false});
	}
	else {
	  this.setState({nameValid: true});
	}
	
	if (!this.state.value) {
	  this.setState({valueValid: false});
	}
	else {
	  this.setState({valueValid: true});
	}
	
	if (!this.state.description) {
	  this.setState({descrValid: false});
	}
	else {
	  this.setState({descrValid: true});
	}
  }
  
  validate() {
	this.isValid();
	
	return this.state.name &&
	       this.state.value &&
		   this.state.description;
  }
  
  handleSubmit(e) {
	e.preventDefault();
	
	if (this.validate()) {
      const formData = {
	    Id:          this.state.id,
	    Name:        this.state.name,
	    Value:       this.state.value,
	    Description: this.state.description
	  };
	
	  this.change('/api/settings', formData)
	    .then(function(result) {
		  if (result.status === true) {
	        window.location = "/#/admin/settings"
		  }
	    })
	    .catch(function(result) {
		  console.log(JSON.stringify(result));  
	    });
	}
  }
  
  render() {
    const { isAuthenticated } = this.props.auth;
	let error  = "form-control text-box single-line";
	let name   = error;
	let descr  = error;
	let value  = error;
	
	if (!this.state.nameValid) {
	  name += " input-error";
	}
	
	if (!this.state.descrValid) {
	  descr += " input-error";
	}
	
	if (!this.state.valueValid) {
	  value += " input-error";
	}
	
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
				  <input type="text" value={this.state.name} onChange={this.updateName} className={name} />
		        </div>
	          </div>
			  
			  <div className="form-group">
		        <label className="control-label col-md-2">Value</label>
		        <div className="col-md-10">
				  <input type="text" value={this.state.value} onChange={this.updateValue} className={value} />
		        </div>
	          </div>

	          <div className="form-group">
		        <label className="control-label col-md-2">Description</label>
		        <div className="col-md-10">
				  <input type="text" value={this.state.description} onChange={this.updateDescription} className={descr} />
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
