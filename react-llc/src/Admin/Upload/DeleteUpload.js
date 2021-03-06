import React, { Component } from 'react';
import servicesBase         from '../../Services/ServicesBase';

class DeleteUpload extends Component {
  constructor(props) {
    super(props);
	this.state = {
      message: null,
	}
  }
	
  componentDidMount() {
	if (this.props.match.params.id) {
	  var that = this;
	  var fd   = new FormData();
	  fd.append('id',     this.props.match.params.id);
	  fd.append('delete', true);
	  this.change('/api/uploads', fd)
	  .then(function(result) {
		if (result.status) {
	      window.location = "/#/admin/upload";
		}
		else {
		  that.setState({message: "Delete Package Error!"});
		}
	  });
	}
  }
  
  render() {
    const { isAuthenticated } = this.props.auth;
    return (
      <div className="container body-content">
        {
          isAuthenticated() && (
              <h3>{this.state.message}</h3>
            )
        }
        
      </div>
    );
  }
}

export default servicesBase(DeleteUpload);
