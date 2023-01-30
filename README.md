# CNYRIC-OneRoster-Client

### This project is based on [jdolny/OneRoster.NET](https://github.com/jdolny/OneRoster.NET/tree/master/OneRoster.NET) 

**uses the following nuGet packages**
- RestSharp
- CsvHelper

**a few tweaks on the original (which is great, BTW)**
* Rearranged/combined classes for conveninece
* Updated some model properties

* In the models, added a method to return row(s) as a comma separated string. Also a header
  * **IMPORTANT: This project does not create csv files. See [ajhardin11/CNYRIC_OneRosterCsv](https://github.com/ajhardin11/CNYRIC_OneRosterCsv) for an example project that creates csv's using this project**
  ###### ex academicSessions header 
      public static void CsvHeader(CsvWriter writer)
		{
			writer.WriteField("sourcedId",true);
			writer.WriteField("status", true);
			writer.WriteField("dateLastModified", true);

			writer.WriteField("title", true);
			writer.WriteField("type", true);
			writer.WriteField("startDate", true);
			writer.WriteField("endDate", true);
			writer.WriteField("parentSourcedId", true);
			writer.WriteField("schoolYear", true);

			writer.NextRecord();
		}
   ###### ex academicSessions row as comma separated string
      public void AsCsvRow(CsvWriter writer, bool bulk = true)
		{
			writer.WriteField(SourcedId, true);

			if (bulk)
			{
				writer.WriteField("", true);
				writer.WriteField("", true);
			}
			else
			{
				writer.WriteField(Status, true);
				writer.WriteField(DateLastModified.ToString(), true);
			}

			writer.WriteField(Title, true);
			writer.WriteField(Type,true);
			writer.WriteField(StartDate.ToString("yyyy-MM-dd"),true);
			writer.WriteField(EndDate.ToString("yyyy-MM-dd"),true);
			writer.WriteField(Parent.SourcedId,true);
			writer.WriteField(SchoolYear, true);
      
			writer.NextRecord();
		}
   
* Added methods to return results as a csv string
  ###### ex get csv string from academicSessions APICall
      public string GetAcademicSessionsCsv(ApiParameters? p = null, bool includeHeader = true	)
		{
			_request.Method = Method.Get;
			_request.Resource = $"/academicSessions/";
			_oneRosterApi.AddRequestParameters(_request, p);
			AcademicSessions Records = _oneRosterApi.Execute<AcademicSessions>(_request);

			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if (includeHeader) AcademicSession.CsvHeader(csv);  //new method to return header
				foreach (AcademicSession Record in Records.academicSessions)
				{
					Record.AsCsvRow(csv); //new method to return row as a comma separated string
				}
				return sb.ToString();
			}
		}
   
    ###### ex get csv string from API Call. Header is automatically included unless specified otherwise
      Request = new V1p1(BaseURL, Client, Password);
      string AcademicSessionscsv=Request.AcademicSessionsManagement.GetAcademicSessionsCsv(CustomRequestParameters);
     
  ###### ex get csv string by passing in academicSessions object. use: to modify/update original results
      public string GetAcademicSessionsCsv(AcademicSessions Records, bool includeHeader = true	)
		{
			var sb = new StringBuilder();
			using (var csv = new CsvWriter(new StringWriter(sb), CultureInfo.InvariantCulture))
			{
				if (includeHeader) AcademicSession.CsvHeader(csv); //new method to return header

				foreach (AcademicSession record in Records.academicSessions)
				{
					record.AsCsvRow(csv); //new method to return row as a comma separated string
				}
				return sb.ToString();
			}
		}
    
     ###### example usage of above that does something with children of an AcademicSession and then gets a csv string without the header
        Request = new V1p1(BaseURL, Client, Password);
        AcademicSessions _academicsessions = Request.AcademicSessionsManagement.GetAllAcademicSessions(CustomRequestParameters);
        
        foreach (AcademicSession a in _academicsessions.academicsessions.FindAll(x => x.Parent == "someguid"))
			{
				//change the parent to a different academic session
        a.Parent="SomeotherGuid"
			}
      //get csv string but don't include the header
      Request.AcademicSessionsManagement.GetAcademicSessionsCsv(_academicsessions,false);

