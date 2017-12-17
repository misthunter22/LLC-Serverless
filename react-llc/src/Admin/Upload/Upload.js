import React, { Component } from 'react';
import servicesBase         from '../../Services/ServicesBase';

const $     = require('jquery');
$.DataTable = require('datatables.net');

const columns = [
  {
	title: 'ID',
	data: 'id'
  },
  {
	title: 'Name',
	data: 'name'
  },
  {
	title: 'File Name',
	data: 'value'
  },
  {
	title: 'Uploaded By',
	data: 'id'
  },
  {
	title: 'Date Uploaded',
	data: 'modified'
  },
  {
	title: 'Package Processed',
	data: 'user'
  }
];

class Upload extends Component {
	
  constructor(props) {
    super(props);
	this.state = {
      packages: [],
      loading: true
	}
  }
  
  componentDidMount() {
	var that = this;
    this.packages()
	  .then(function(packages) {
		that.setState({packages: packages});
		that.changeSpinner(that, false);
		
		$(that.refs.main).DataTable({
          dom: '<"data-table-wrapper"t>',
          data: packages,
          columns,
          ordering: false,
          columnDefs: [
          {
            "render": function (data, type, row) {
	          return '<a title="' + data + '"><span class="badge truncate">' + data + '</span></a>';
            },
		    "width": "100px",
            "targets": 0
          },
          {
            "render": function (data, type, row) {
              return '<a href="/admin/upload/manage/' + data +'" title="Edit this setting.">' +
                       '<i class="glyphicon glyphicon-pencil"></i>' +
                         '<span class="sr-only">Edit</span>' +
                     '</a>';
              },
              "targets": 1
          },
          {
	        "render": function (data, type, row) {
	          return '<span onclick="return confirm(\'Are you sure you wish to delete this setting? There is no undo.\')">' +
				       '<a href="/admin/upload/delete/' + data + '" title="Remove this setting.">' +
				         '<i class="glyphicon glyphicon-remove" style="color: red;"></i>' +
				         '<span class="sr-only">Remove</span>' +
				       '</a>' +
			         '</span>';
	        },
	        "targets": 2
          }
	      ]
        });
	  });
  }
  
  componentWillUnmount(){
    $('.data-table-wrapper')
      .find('table')
      .DataTable()
      .destroy(true);
  }
  
  shouldComponentUpdate() {
	if (this.state.packages.length > 0) {
	  const table = $('.data-table-wrapper')
                      .find('table')
                      .DataTable();
					
      table.clear();
      table.rows.add(this.state.packages);
	  table.columns.adjust();
      table.draw();
	  
	  this.changeSpinner(this, false);
	}
	
	return false;
  }
  
  render() {
	let spinner = this.spinnerMarkup();
    const { isAuthenticated } = this.props.auth;
    return (isAuthenticated() && (
		    <div className="container body-content">
              <h2 className="bottom-20">Upload</h2>
              <p>
                <a href="/admin/upload/manage">Create New</a>
              </p>
			  {spinner}
			  <table className="table" ref="main" />
              <div className="bottom-20">
                <a href="/admin">Back to Dashboard</a>
              </div>
            </div>));
  }
}

export default servicesBase(Upload);
