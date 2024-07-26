for i = 0,4,1 do
	MarbleGame:Multiply(0.8, 1.5)
end

while true do
	MarbleGame:Multiply(0.7, 1.1)

	-- bonus
	if MarbleGame.AliveTeams.Count >= 2 and MarbleGame.TotalPopulation <= 100 then
		local index = math.random(0, MarbleGame.Teams.Count - 1)
		MarbleGame:Multiply(1.5, 2.0 + MarbleGame.Teams[index].Population / MarbleGame.TotalPopulation, MarbleGame.Teams[index])
	end

	-- tenbatsu
	local draw = math.random()
	if draw <= 0.04 then
		local index = math.random(MarbleGame.AliveTeams.Count - 1)
		MarbleGame:Set(5, 10, MarbleGame.AliveTeams[index])
	end

	-- end game quicker
	if MarbleGame.AliveTeams.Count == 1 then
		MarbleGame:Multiply(0.4, 1.0)
	end
end