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
	data: 'fileName'
  },
  {
	title: 'Date Uploaded',
	data: 'dateUploaded'
  },
  {
	title: 'Package Processed',
	data: 'packageProcessed'
  },
  {
	title: '',
	data: 'id'
  }
];

class Upload extends Component {
	
  constructor(props) {
    super(props);
	this.state = {
	  url     : "",
      pckgs   : [],
      loading : true
	}
  }
  
  componentDidMount() {
	var that = this;
    this.packages()
	  .then(function(pckgs) {
		that.setState({pckgs: pckgs});
		that.changeSpinner(that, false);
		
		$(that.refs.main).DataTable({
          dom: '<"data-table-wrapper"t>',
          data: pckgs,
          columns,
          ordering: false,
          columnDefs: [
          {
            "render": function (data, type, row) {
	          return '<a target="_blank" href="/admin/upload/file/' + data + '" title="' + data + '"><span class="badge truncate">' + data + '</span></a>';
            },
		    "width": "100px",
            "targets": 0
          },
          {
            "render": function (data, type, row) {
              return data + '&nbsp;' +
                '<i class="glyphicon glyphicon-info-sign" title="' + row['description'] + '"></i>';
              },
              "targets": 1
          },
          {
	        "render": function (data, type, row) {
	          return data + '&nbsp;' +
                '<i class="glyphicon glyphicon-info-sign" title="' + row['key'] + '"></i>';
	        },
	        "targets": 2
          },
		  {
            "render": function (data, type, row) {
	          return '<span onclick="return confirm(\'Are you sure you wish to delete this package? There is no undo.\')">' +
                       '<a href="/admin/upload/delete/' + data + '" title="Remove this package.">' +
                         '<i class="glyphicon glyphicon-remove" style="color: red;"></i>' +
                         '<span class="sr-only">Remove</span>' +
                       '</a>' +
                     '</span>';
            },
            "targets": 5
          }
	      ]
        });
	  });
	  
	this.packageUrl().then(function(url) {
	  $("#packageAws").attr("href", url.value)
	});
  }
  
  componentWillUnmount(){
    $('.data-table-wrapper')
      .find('table')
      .DataTable()
      .destroy(true);
  }
  
  shouldComponentUpdate() {
	if (this.state.pckgs.length > 0) {
	  const table = $('.data-table-wrapper')
                      .find('table')
                      .DataTable();
					
      table.clear();
      table.rows.add(this.state.pckgs);
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
			  {/*<a href="/admin/upload/manage">Create New</a>*/}
			    <a id="packageAws" target="_blank">New uploads are sent directly to S3</a>
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
