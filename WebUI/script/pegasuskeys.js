
function GetWebKey()
{
	var webKey
	
	if (window.location.protocol.toLowerCase() == "http:")
	{
		if (window.location.toLowerCase().indexOf("localhost") > -1)
		{
			webKey = "localhost?C0D75518493F4FD41777E366C3EDE4FC";
		}
		else
		{
			if (window.location.toLowerCase().indexOf("hmstest") > -1)
			{
				webKey = "hmstest.jackrichards.co.uk?A4358745B6B0AC2F7700070E51FD5E98";
			}
			else
			{
				webKey = "hms.jackrichards.co.uk?D196E6D44F66C9D4CF18EC83D36113E0";
			}
		}
	}
	
	return webKey;
}