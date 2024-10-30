IF 151 > (SELECT COUNT(*) FROM Region)
BEGIN
DELETE FROM [Region]

--- East Midlands 9
INSERT INTO [dbo].[Region] ([SubregionName],[RegionName],[Latitude],[Longitude]) 
		VALUES ('Derby','East Midlands',52.9219,-1.47564),
		 ('Derbyshire','East Midlands',52.9219,-1.47564),
		 ('Leicester','East Midlands',52.63486,-1.12906),
		 ('Leicestershire','East Midlands',52.63486,-1.12906),
		 ('Lincolnshire','East Midlands',52.9452,-0.1601),
		 ('Northamptonshire','East Midlands',52.23484,-0.89732),
		 ('Nottingham','East Midlands',52.95512,-1.14917),
		 ('Nottinghamshire','East Midlands',52.95512,-1.14917),
		 ('Rutland','East Midlands',52.6583,-0.6396)

-- East of England -11
INSERT INTO [dbo].[Region] ([SubregionName],[RegionName],[Latitude],[Longitude]) 
		VALUES ('Bedford','East of England',52.13571,-0.46804),
		('Cambridgeshire','East of England',52.57339,-0.24846),
		('Central Bedfordshire','East of England',52.00268,-0.29749),
		('Essex','East of England',51.5742,-0.4857),
		('Hertfordshire','East of England',51.8098,-0.2377),
		('Luton','East of England',51.87965,-0.41756),
		('Norfolk','East of England',52.614,0.8864),
		('Peterborough','East of England',52.57339,-0.24846),
		('Southend-on-Sea','East of England',51.54041,0.7077),
		('Suffolk','East of England',52.1872,0.9708),
		('Thurrock','East of England',51.4935,0.3529)

--London -32 (+Enfield)
INSERT INTO [dbo].[Region] ([SubregionName],[RegionName],[Latitude],[Longitude]) 
		VALUES ('Barking and Dagenham','London',51.53628,0.08148),
		('Barnet','London',51.65293,-0.19961),
		('Bexley','London',51.44135,0.14861),
		('Brent','London',51.5673,-0.2711),
		('Bromley','London',51.40568,0.01435),
		('Camden','London',51.5517,-0.1588),
		('City of London','London',51.53628,0.08148),
		('Croydon','London',51.37236,-0.0982),
		('Ealing','London',51.5133,-0.08153),
		('Greenwich','London',51.4934,0.0098),
		('Hackney','London',51.5734,-0.0724),
		('Hammersmith and Fulham','London',51.499,-0.2291),
		('Haringey','London',51.5906,-0.111),
		('Harrow','London',51.57881,-0.33376),
		('Havering','London',51.5779,0.2121),
		('Hillingdon','London',51.53358,-0.45258),
		('Hounslow','London',51.46759,-0.3618),
		('Islington','London',51.5465,-0.1058),
		('Kensington and Chelsea','London',51.4991,-0.1938),
		('Kingston upon Thames','London',51.41232,-0.30044),
		('Lambeth','London',51.4571,-0.1231),
		('Lewisham','London',51.4415,-0.0117),
		('Merton','London',51.4098,-0.2108),
		('Newham','London',51.5255,0.0352),
		('Redbridge','London',51.5901,0.0819),
		('Richmond upon Thames','London',51.4613,-0.30044),
		('Southwark','London',51.5028,-0.0877),
		('Sutton','London',51.36045,-0.19178),
		('Tower Hamlets','London',51.5203,-0.0293),
		('Waltham Forest','London',51.5886,-0.0118),
		('Wandsworth','London',51.4571,-0.1818),
		('Westminster','London',51.4975,-0.1357),
		('Enfield','London',51.6521,-0.08153)

-- North East 12
INSERT INTO [dbo].[Region] ([SubregionName],[RegionName],[Latitude],[Longitude]) 
		VALUES ('County Durham','North East',54.77869,-1.55961),
		('Darlington','North East',54.52873,-1.5595),
		('Gateshead','North East',54.95937,-1.60182),
		('Hartlepool','North East',54.68249,-1.2167),
		('Middlesbrough','North East',54.57301,-1.23791),
		('Newcastle upon Tyne','North East',54.97784,-1.61292),
		('North Tyneside','North East',55.0182,1.4858),
		('Northumberland','North East',55.2083,-2.0784),
		('Redcar and Cleveland','North East',54.60301,-1.07763),
		('South Tyneside','North East',54.9637,-1.4419),
		('Stockton-on-Tees','North East',54.56823,-1.31443),
		('Sunderland','North East',54.90445,-1.38145)

--  North West 23
INSERT INTO [dbo].[Region] ([SubregionName],[RegionName],[Latitude],[Longitude]) 
		VALUES ('Blackburn with Darwen','North West',53.7501,-2.48471),
		('Blackpool','North West',53.81418,-3.05354),
		('Bolton','North West',53.57846,-2.42984),
		('Bury','North West',53.59346,-2.29854),
		('Cheshire East','North West',53.161,-2.2186),
		('Cheshire West and Chester','North West',53.21744,-2.74297),
		('Cumbria','North West',54.5772,-2.7975),
		('Halton','North West',53.3613,-2.7335),
		('Knowsley','North West',53.4546,-2.8529),
		('Lancashire','North West',53.54125,-2.11766),
		('Liverpool','North West',53.41078,-2.97784),
		('Manchester','North West',53.48071,-2.23438),
		('Oldham','North West',53.54125,-2.11766),
		('Rochdale','North West',53.61635,-2.15871),
		('Salford','North West',53.4875,-2.2901),
		('Sefton','North West',53.5034,-2.9704),
		('St Helens','North West',53.45388,-2.73689),
		('Stockport','North West',53.40849,-2.14929),
		('Tameside','North West',53.4806,-2.081),
		('Trafford','North West',53.4707,-2.3231),
		('Warrington','North West',53.39266,-2.587),
		('Wigan','North West',53.54427,-2.63106),
		('Wirral','North West',53.3727,-3.0738)

-- South East 19
INSERT INTO [dbo].[Region] ([SubregionName],[RegionName],[Latitude],[Longitude]) 
		VALUES ('Bracknell Forest','South East',51.4154,-0.7536),
		('Brighton and Hove','South East',50.83022,-0.1372),
		('Buckinghamshire','South East',51.8137,-0.8095),
		('East Sussex','South East',50.9086,-0.2494),
		('Hampshire','South East',51.0577,-1.3081),
		('Isle of Wight','South East',50.6938,-1.3047),
		('Kent','South East',51.2787,0.5217),
		('Medway','South East',51.4047,0.5418),
		('Milton Keynes','South East',52.04144,-0.76056),
		('Oxfordshire','South East',51.75374,-1.26346),
		('Portsmouth','South East',50.79891,-1.09116),
		('Reading','South East',51.45504,-0.9781),
		('Slough','South East',51.50935,-0.59545),
		('Southampton','South East',50.90497,-1.40323),
		('Surrey','South East',51.3148,-0.56),
		('West Berkshire','South East',51.4308,-1.1445),
		('West Sussex','South East',50.83664,-0.4617),
		('Windsor and Maidenhead','South East',51.48467,-0.64786),
		('Wokingham','South East',51.41097,-0.83493)

-- South West 16
INSERT INTO [dbo].[Region] ([SubregionName],[RegionName],[Latitude],[Longitude]) 
		VALUES ('Bath and North East Somerset','South West',51.34762,-2.4766),
		('Bournemouth','South West',50.72168,-1.87853),
		('Bristol','South West',51.4545,-2.5879),
		('Cornwall','South West',50.266,-5.0527),
		('Devon','South West',50.7156,-3.5309),
		('Dorset','South West',50.7488,-2.3445),
		('Gloucestershire','South West',51.86674,-2.24867),
		('Isles of Scilly','South West',49.9146,-6.31574),
		('North Somerset','South West',51.3879,-2.7781),
		('Plymouth','South West',50.37038,-4.14265),
		('Poole','South West',50.71939,-1.98114),
		('Somerset','South West',51.1051,-2.9262),
		('South Gloucestershire','South West',51.5264,-2.4728),
		('Swindon','South West',51.55842,-1.78204),
		('Torbay','South West',50.4619,-3.5253),
		('Wiltshire','South West',51.3492,-1.9927)

-- West Midlands 14
INSERT INTO [dbo].[Region] ([SubregionName],[RegionName],[Latitude],[Longitude])
		VALUES ('Birmingham','West Midlands',52.4829,-1.89346),
		('Coventry','West Midlands',52.40631,-1.50852),
		('Dudley','West Midlands',52.50867,-2.08734),
		('Herefordshire','West Midlands',52.0765,-2.6544),
		('Sandwell','West Midlands',52.50636,-1.96258),
		('Shropshire','West Midlands',52.67587,-2.4497),
		('Solihull','West Midlands',52.41471,-1.7743),
		('Staffordshire','West Midlands',52.8793,-2.0572),
		('Stoke-on-Trent','West Midlands',53.02578,-2.17739),
		('Telford and Wrekin','West Midlands',52.741,-2.4869),
		('Walsall','West Midlands',52.58595,-1.98229),
		('Warwickshire','West Midlands',52.28194,-1.58447),
		('Wolverhampton','West Midlands',52.58533,-2.13192),
		('Worcestershire','West Midlands',52.19204,-2.22353)

-- Yorkshire and The Humber 15
INSERT INTO [dbo].[Region] ([SubregionName],[RegionName],[Latitude],[Longitude])
		VALUES ('Barnsley','Yorkshire and The Humber',53.55293,-1.48127),
		('Bradford','Yorkshire and The Humber',53.79385,-1.75244),
		('Calderdale','Yorkshire and The Humber',53.7248,-1.8658),
		('Doncaster','Yorkshire and The Humber',53.52304,-1.13376),
		('East Riding of Yorkshire','Yorkshire and The Humber',53.84292,-0.42766),
		('Kingston upon Hull','Yorkshire and The Humber',53.74434,-0.33244),
		('Kirklees','Yorkshire and The Humber',53.5933,-1.801),
		('Leeds','Yorkshire and The Humber',53.79969,-1.5491),
		('North East Lincolnshire','Yorkshire and The Humber',53.5668,-0.0815),
		('North Lincolnshire','Yorkshire and The Humber',53.6056,-0.5597),
		('North Yorkshire','Yorkshire and The Humber',53.9915,-1.5412),
		('Rotherham','Yorkshire and The Humber',53.4302,-1.35685),
		('Sheffield','Yorkshire and The Humber',53.38306,-1.46479),
		('Wakefield','Yorkshire and The Humber',53.68297,-1.4991),
		('York','Yorkshire and The Humber',53.96,-1.0873)
END

if not exists(select * from Region where SubregionName='Enfield')
	BEGIN
		INSERT INTO [dbo].[Region] ([SubregionName],[RegionName],[Latitude],[Longitude]) 
			VALUES ('Enfield','London',51.6521,-0.08153)
	END

update [dbo].[Region] set Longitude = -1.5595 where subregionName = 'Darlington' and longitude = '-54.52873'