AddEventHandler('chatMessage', function(from, name, message)
	if message == '/speed' then
		TriggerClientEvent('speed:spawn', from)
	end
end)