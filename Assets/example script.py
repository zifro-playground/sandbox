# Inställningar
hastighet = 5
rotation = 3
gravitation = 1
hopp = 5

# Variabler
nedåtHastighet = 0
kör = True

# Main loop
while kör:
	if not blockerad_ner():
		nedåtHastighet = nedåtHastighet + gravitation
		if nedåtHastighet > 10:
			kör = False
	elif knapp_mellanslag():
		nedåtHastighet = -hopp
		
	gå_ner_blockerad(nedåtHastighet)

	if knapp_höger():
		sväng_höger(rotation)
	if knapp_vänster():
		sväng_vänster(rotation)

	if knapp_upp():
		gå_framåt_blockerad(hastighet)
	if knapp_ner():
		gå_bakåt_blockerad(hastighet)
