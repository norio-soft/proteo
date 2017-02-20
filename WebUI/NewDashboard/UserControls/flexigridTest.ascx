<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="flexigridTest.ascx.cs" Inherits="Orchestrator.WebUI.NewDashboard.UserControls.flexigridTest" %>
<script type='text/javascript' src='/NewDashboard/Scripts/flexigrid-1.1/js/flexigrid.js'></script>
<script type='text/javascript' src='/NewDashboard/Scripts/flexigrid-1.1/js/flexigrid.pack.js'></script>
<link rel="stylesheet" type="text/css" href="/NewDashboard/Scripts/flexigrid-1.1/css/flexigrid.pack.css" />
<link rel="stylesheet" type="text/css" href="/NewDashboard/Scripts/flexigrid-1.1/css/flexigrid.css" />

<table class="flexme2">
		<thead>
			<tr>
				<th width="100">Col 1</th>
				<th width="100">Col 2</th>
				<th width="100">Col 3 is a long header name</th>
				<th width="300">Col 4</th>
			</tr>
		</thead>
		<tbody>
			<tr>
				<td>This is data 1 with overflowing content</td>
				<td>This is data 2</td>
				<td>This is data 3</td>
				<td>This is data 4</td>
			</tr>
			<tr>
				<td>This is data 1</td>
				<td>This is data 2</td>
				<td>This is data 3</td>
				<td>This is data 4</td>
			</tr>
			<tr>
				<td>This is data 1</td>
				<td>This is data 2</td>
				<td>This is data 3</td>
				<td>This is data 4</td>
			</tr>
			<tr>
				<td>This is data 1</td>
				<td>This is data 2</td>
				<td>This is data 3</td>
				<td>This is data 4</td>
			</tr>
			<tr>
				<td>This is data 1</td>
				<td>This is data 2</td>
				<td>This is data 3</td>
				<td>This is data 4</td>
			</tr>
			<tr>
				<td>This is data 1</td>
				<td>This is data 2</td>
				<td>This is data 3</td>
				<td>This is data 4</td>
			</tr>
		</tbody>
	</table>

    <script type="text/javascript">
        $('.flexme2').flexigrid({striped: false, height: $('flexigridTest').height(), width: $('flexigridTest').width()});
</script>