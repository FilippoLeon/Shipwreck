#! /usr/bin/env lua
-- 

function buildBuildingPlacementPanel(UI, verse)
	panel = UI["creative_panel"]
	print(verse.Name)
	for key, prototype in pairs(verse.registry.partRegistry.prototypes)
	do
		print(prototype.Id)
		button = Button.Create(prototype.Id)
		panel.Add(button)
		button.Text = prototype.Id
		-- button.TextColor = Color.black
		button.SetPreferredSize(100)
		function printID()
			print("Building : " .. prototype.Id)
			verse.SetMode(1, { prototype.Id });
		end
		button.OnClick(printID)
	end
end
