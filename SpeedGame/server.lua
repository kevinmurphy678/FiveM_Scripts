RegisterServerEvent('speed:scoreServer')
AddEventHandler('speed:scoreServer', function(name, time)
	TriggerClientEvent('speed:scoreClient',-1, name, time)
end)


AddEventHandler('chatMessage', function(from, name, message)
	if message == '/speed' then
		TriggerClientEvent('speed:spawn', from)
	end
end)

