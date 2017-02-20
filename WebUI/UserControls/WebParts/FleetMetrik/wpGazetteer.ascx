<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="wpGazetteer.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.WebParts.FleetMetrik.wpGazetteer" %>

<table id="gazetteer" class="display" cellspacing="0" width="100%">
    <thead>
        <tr>
            <th>Reg</th>
            <th>Driver</th>
            <th>Where</th>
            <th>When</th>
        </tr>
    </thead>
</table>

<script>
    $(document).ready(function () {

        var getData = apiService.get('<%=this.DataUrl%>');

        getData.done(function (dataSet) {
            $('#gazetteer').dataTable({
                paging: false,
                scrollY: 168,
                data: dataSet,
                columns: [
                    { data: 'regNo' },
                    {
                        data: null, render: function (data, type, full) {
                            if (full['firstNames'] == null || full['lastName'] == null)
                                return '';
                            else
                                return full['firstNames'] + ' ' + full['lastName'];
                    }
                    },
                    { data: 'locationString' },
                    {
                        data: null, render: function (data, type, full) {
                            var date = moment(full['dateStamp']);
                            return date.format('DD/MM HH:mm');
                        }
                    }
                ]
            });
        });

        
    });
</script>