<%--<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Sample1.Models.Customer>" %>--%>
<%@ Import Namespace="Sample1.Models" %>

<?xml version="1.0" encoding="UTF-8" ?>
<itext creationdate="2/4/2003 5:49:07 PM" producer="iTextSharpXML">
	<paragraph leading="18.0" font="unknown" size="16.0" align="Default">
	    <chunk>Orders in PDF hello world</chunk>
	</paragraph>
	<paragraph leading="18.0" font="unknown" size="10.0" align="Default">
	<%--	<chunk>Customer Name: <%= this.Model.Name %></chunk><newline />
		<chunk>Address: <%= this.Model.Address %></chunk><newline />--%>
	</paragraph>
	<paragraph leading="18.0" font="unknown" size="10.0" align="Default">
	<chunk font="unknown" size="12.0">Orders:</chunk><newline />
	<%--<% foreach (Order o in this.Model.Order)
       { %>
        <chunk font="unknown" size="10.0"><%= o.OrderId %>, <%= o.Description %>, <%= o.Amount %>, <%= o.Date %></chunk><newline />
	<% } %>--%>
	</paragraph>
</itext>
