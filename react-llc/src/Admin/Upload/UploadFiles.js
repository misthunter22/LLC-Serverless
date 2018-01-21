import React, { Component } from 'react';
import servicesBase         from '../../Services/ServicesBase';
import $                    from 'jquery';

window.jQuery = $;
window.$ = $;
global.jQuery = $;

require('bootstrap');
require('datatables.net');
require('datatables-bootstrap');

const columns = [
  {
	title: 'ID',
	data: 'id'
  },
  {
	title: 'Course Location',
	data: 'courseLocation'
  },
  {
	title: 'Link',
	data: 'link'
  },
  {
	title: 'Protocol',
	data: 'protocol'
  },
  {
	title: 'Link Name',
	data: 'linkName'
  },
  {
	title: 'Parent Folder',
	data: 'parentFolder'
  }
];

class UploadFiles extends Component {
	
  constructor(props) {
    super(props);
	this.state = {
      files   : [],
      loading : true
	}
  }
  
  componentDidMount() {
	var that = this;
	var id   = this.props.match.params.id;
    this.packageFiles(id)
	  .then(function(files) {
		  
		that.setState({files: files});
		that.changeSpinner(that, false);
		
		$(that.refs.main).DataTable({
          columns,
	      dom: 'lfrtip',
          ordering: true,
	      processing: false,
          serverSide: false,
	      stateSave: true,
	      autoWidth: false,
	      lengthChange: true,
          filter: true,
          info: true,
		  data: files,
		  columnDefs: [
          {
            render: function (data, type, row) {
              return '<span class="hyphenate dont-break-out">' + data + '</span>';
            },
            targets: 1
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
	if (this.state.files.length > 0) {
	  const table = $('.data-table-wrapper')
                      .find('table')
                      .DataTable();
					
      table.clear();
      table.rows.add(this.state.files);
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
              <h2 className="bottom-20">Upload Files</h2>
			  {spinner}
			  <table width="100%" className="table table-striped table-bordered" ref="main" cellSpacing="0">
			  </table>
              <div className="bottom-20">
                <a href="/admin/upload">Back to Dashboard</a>
              </div>
            </div>));
  }
}

export default servicesBase(UploadFiles);
