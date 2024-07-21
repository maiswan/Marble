lastIndex = -1
index = -1

for i = 0,4,1 do
	MarbleGame:Multiply(0.8, 1.2)
end

while true do
	MarbleGame:Multiply(1.0, 1.2)
	MarbleGame:Multiply(0.5, 0.9)

	-- bonus
	if MarbleGame.TeamsAlive >= 2 and MarbleGame.TeamsAlive <= 4 then
		repeat
			index = math.random(0, MarbleGame.Teams.Count - 1)
		until MarbleGame.Teams[index].Population ~= 0 and index ~= lastIndex

		MarbleGame:Set(index, MarbleGame.Teams[index].Population * 2)
		lastIndex = index
	end

	-- end game quicker
	if MarbleGame.TeamsAlive == 1 then
		MarbleGame:Multiply(0.5, 1.0)
	end
end