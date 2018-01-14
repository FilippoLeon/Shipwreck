#! /usr/bin/env lua
-- 

function buildCreativePlacementPanel(UI, verse)
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

function buildOverlayPanel(UI, verse)
	panel = UI["overlay_panel"]
	button = Button.Create("none")
	panel.Add(button)
	button.Text = "None"
	button.SetPreferredSize(50)
	function setMap()
		print("Disabling overlay map.")
		verse.SetMap(nil);
	end
	button.OnClick(setMap)
	for key, prototype in pairs(verse.maps)
	do
		print(key)
		button = Button.Create(key)
		panel.Add(button)
		button.Text = key
		button.SetPreferredSize(50)
		function setMap()
			print("Setting overlay map to: " .. key)
			verse.SetMap(key);
		end
		button.OnClick(setMap)
	end
end