Function GetWebKey()
	dim WebKey
	if Location.protocol = "http:" then 
		if (instr(1, window.location, "localhost", 1) > 0) then 
			WebKey = "localhost/cpashare?3F6E9571F1384BB21B612FE1660B1539"
		else
			if (instr(1, window.location, "nwmhp", 1) > 0) then
				WebKey = "nww.sid.nwmhp.nhs.uk/cpashare?0A30206A2779ACA500D635DC48714885"
			else
				if (instr(1, window.location, "/Scan/", 1) > 0) then
					WebKey = "Scan?14F3E598BC6C199A204B37A3DB4A015A"
				else 
					if (instr(1, window.location, "SID", 1) > 0) then
						webKey	= "SID?559897CFC159BF2D1AC15E320C5ADB8F"
					end if
				End If
			End if
		end if
	Else
		if (instr(1, window.location, "localhost", 1) > 0) then 
			WebKey = "https://localhost?A848844AC277F9B8824BCE426F7F54E8"
		else
			if (instr(1, window.location, "nwmhp", 1) > 0) then
				WebKey = "https://nww.sid.nwmhp.nhs.uk/cpashare?5924E971988F041FE175A1C0520378B1"
			else
				if (instr(1, window.location, "/Scan/cpashare", 1) > 0) then
					WebKey = "https://scan?15CB5615575DF23EDF72267103A08667"
				else
					if (instr(1, window.location, "SID", 1) > 0) then
						webKey	= "https://sid?B9955DAED9A168E74054ECA26A8D90FA"
					end if
				End If
			End if
		end if
	end If
	'MsgBox(WebKey)
	GetWebKey = WebKey
End Function