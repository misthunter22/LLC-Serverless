import React, { Component } from 'react';
import servicesBase         from '../../Services/ServicesBase';

const $     = require('jquery');
$.DataTable = require('datatables.net');

const columns = [
  {
	title: 'ID',
	data: 'source'
  },
  {
	title: 'Name',
	data: 'title'
  },
  {
	title: 'Allow Link Checking',
	data: 'allowlink'
  },
  {
	title: 'Allow Link Extracting',
	data: 'allowextract'
  },
  {
	title: 'Object Source Name',
	data: 's3name'
  },
  {
	title: '',
	data: 'source'
  },
  {
	title: 'Date Created',
	data: 'created'
  },
  {
	title: '',
	data: 'source'
  }
];

class Sources extends Component {
	
  constructor(props) {
    super(props);
	this.state = {
      sources: [],
	  loading: true
	}
  }
  
  componentDidMount() {
    this.sources(true);
	$(this.refs.main).DataTable({
      dom: '<"data-table-wrapper"t>',
      data: this.state.sources,
      columns,
      ordering: false,
	  columnDefs: [
	  {
        "render": function (data, type, row) {
		  return '<a href="/admin/sources/editsource/' + data +'" title="Edit this source.">' +
                   '<i class="glyphicon glyphicon-pencil"></i>' +
                   '<span class="sr-only">Edit</span>' +
                 '</a>';
        },
        "targets": 5
      },
      {
        "render": function (data, type, row) {
	      return '<span onclick="return confirm(\'Are you sure you wish to delete this source? There is no undo.\')">' +
                   '<a href="/admin/sources/deletesource/' + data + '" title="Remove this source.">' +
                     '<i class="glyphicon glyphicon-remove" style="color: red;"></i>' +
                     '<span class="sr-only">Remove</span>' +
                   '</a>' +
                 '</span>';
        },
        "targets": 7
      }
      ]
    });
  }
  
  componentWillUnmount(){
    $('.data-table-wrapper')
      .find('table')
      .DataTable()
      .destroy(true);
  }
  
  shouldComponentUpdate() {
	if (this.state.sources.length > 0) {
	  const table = $('.data-table-wrapper')
                      .find('table')
                      .DataTable();
					
      table.clear();
      table.rows.add(this.state.sources);
	  table.columns.adjust();
      table.draw();
	  
      this.turnOffSpinner(this);
	}
	
	return false;
  }
  
  render() {
	let spinner = this.spinnerMarkup();
    const { isAuthenticated } = this.props.auth;
    return (isAuthenticated() && (
		    <div className="container body-content">
              <h2 className="bottom-20">Sources</h2>
              <p>
                <a href="/admin/sources/createsource">Create New</a>
              </p>
			  {spinner}
			  <table className="table" ref="main" />
              <div className="bottom-20">
                <a href="/admin">Back to Dashboard</a>
              </div>
            </div>));
  }
}

export default servicesBase(Sources);
