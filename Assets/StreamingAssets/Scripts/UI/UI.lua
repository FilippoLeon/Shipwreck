#! /usr/bin/env lua
-- 

function buildCreativePlacementPanel(UI, verse)
	panel = UI["creative_panel"]
	-- print(verse.Name)
	for key, prototype in pairs(verse.registry.partRegistry.prototypes)
	do
		-- print(prototype.Id)
		button = Button.Create(prototype.Id)
		panel.AddChild(button)
		button.Text = prototype.Id
		-- button.TextColor = Color.black
		button.SetPreferredSize(150)
		function printID()
			-- print("Building : " .. prototype.Id)
			verse.SetMode(1, { prototype.Id });
		end
		button.OnClick(printID)
	end
end

function buildOverlayPanel(UI, verse)
	panel = UI["overlay_panel"]
	button = Button.Create("none")
	panel.AddChild(button)
	button.Text = "None"
	button.SetPreferredSize(100)
	function setMap()
		-- print("Disabling overlay map.")
		verse.SetMap(nil);
	end
	button.OnClick(setMap)
	for key, prototype in pairs(verse.maps)
	do
		-- print(key)
		button = Button.Create(key)
		panel.AddChild(button)
		button.Text = key
		button.SetPreferredSize(100)
		function setMap()
			-- print("Setting overlay map to: " .. key)
			verse.SetMap(key);
		end
		button.OnClick(setMap)
	end
end

function rebuildMerchantPanel(UI, verse, this)
	-- Merchant
	-- print(UI["merchant_view"].GetParameter("@merchant").Name)
	-- Verse.Instance.ActiveShip().Location.GetNpc(0)
    -- print(UI["merchant_view"]["merchant_inventory"])
	merchant = this.GetParameter("@merchant")
	player = verse.Player
	trade = merchant.GetTrade(verse.Player.Inventory)
	UI["merchant_view"]["merchant_inventory"].SetScrollSpeed(30)
	sell_inv = trade.GetSellInventory()
	
	UI["merchant_view"]["merchant_inventory"].Clear()
	
	-- Merchant details
	l_cost = Label.Create()
	l_cost.Text = "0"
	UpdateMerchant = function ()
		l_cost.Text = "Cost: " .. trade.GetCost() .. "$"
	end
	l_funds = Label.Create()
	l_mfunds = Label.Create()
	local update_funds_text = function()
		l_funds.Text = "Merchant: " .. trade.GetSellInventory().Funds .. "$"
		l_mfunds.Text = "Available: " .. trade.GetBuyInventory().Funds .. "$"
	end
	update_funds_text()
	b_reset = Button.Create()
	b_reset.Text = "Reset"
	b_reset.SetMinSize(100,-1)
	b_finalize = Button.Create()
	b_finalize.Text = "Confirm"
	b_finalize.SetMinSize(100,-1)
	-- Extra info panel
	p_details = UI["merchant_view"]["merchant_details"]
	p_details.Clear()
	p_details.AddChild(l_mfunds)
	p_details.AddChild(l_cost)
	p_details.AddChild(l_funds)
	p_details.AddChild(b_reset)
	p_details.AddChild(b_finalize)
	
	UI["merchant_view"]["merchant_header"].Clear()
	header = UI["merchant_view"]["merchant_header"].SetLayout("horizontal").AddChilds( {
		Label.Create().SetText("").SetMinSize(64).SetAlignment("left"),
		Label.Create().SetText("Part name").SetMinSize(146).SetAlignment("left"),
		Label.Create().SetText("Buy").SetMinSize(122).SetAlignment("left"),
		Label.Create().SetText("Basket").SetMinSize(122).SetAlignment("left"),
		Label.Create().SetText("Sell").SetMinSize(30).SetAlignment("left"),
		Label.Create().SetText("Cost").SetMinSize(100).SetAlignment("left")
	} )
	
	ut_map = {}
	AddPartPanel = function(part, item)
		-- print(item)
		local p = Panel.Create()
			.SetLayout("horizontal").SetPadding({3,3,3,3})
			.SetMinSize(100,20).SetMargin(30)
			.AddTo(UI["merchant_view"]["merchant_inventory"])
		
		-- Item icon 
		local i_icon = Image.Create().SetSprite(part.SpriteInfo.Get(0)).AddTo(p)
		
		-- Item name label
		local l_item = Label.Create()
			.SetAlignment("left")
			.SetText(part.Name)
			.SetMinSize(100,-1)
		
		-- Buy inventory Label
		local l_buy = Label.Create().SetMinSize(30, -1)
		local UpdateText = function()
			if trade.GetBuyInventory().Contains(part) then 
				l_buy.Text = trade.GetBuyInventory().Get(part).quantity
			else
				l_buy.Text = "0"
			end
		end
		table.insert(ut_map, UpdateText)
		UpdateText()
		
		-- Sell inventory label
		local l_sell = Label.Create().SetMinSize(30, -1)
		local UpdateText = function()
			if sell_inv.Contains(part) then
				sitem = sell_inv.Get(part)
				l_sell.Text = sitem.quantity
			else
				l_sell.Text = "0"
			end
		end
		table.insert(ut_map, UpdateText)
		UpdateText()
		
		-- Basket quantity label
		local l_basket = Label.Create().SetText(0).SetMinSize(30, -1)
		local UpdateText = function()
			if trade.InBasket(part) then
				l_basket.Text = trade.Basket(part).quantity
			else
				l_basket.Text = 0
			end
		end
		table.insert(ut_map, UpdateText)
		
		-- Sell button
		local sell_b = Button.Create()
		sell_b.SetType("simple")
		sell_b.SetSprite(SpriteInfo.__new("UI", "minus"))
		sell_b.SetNonExpanding()
		local sell = function() 
			trade.AddToBasket(part, -1)
			UpdateText()
			UpdateMerchant()
		end
		sell_b.OnClick(sell)
		sell_b.Tooltip = "Sell 1"
		
		-- Buy button
		local buy_b = Button.Create()
		buy_b.SetType("simple")
		buy_b.SetSprite(SpriteInfo.__new("UI", "plus"))
		buy_b.SetNonExpanding()
		local buy = function() 
			trade.AddToBasket(part, Input.LShift and 10 or 1)
			UpdateText()
			UpdateMerchant()
		end
		buy_b.OnClick(buy)
		buy_b.Tooltip = "Buy 1\nClick + <b>Shift:</b> Buy 10"
		
		-- Sell all button
		local sell_all_b = Button.Create()
		sell_all_b.SetType("simple")
		sell_all_b.SetSprite(SpriteInfo.__new("UI", "minus_all"))
		sell_all_b.SetNonExpanding()
		local sell_all = function() 
			trade.AddToBasket(part, -1000)
			UpdateText()
			UpdateMerchant()
		end
		sell_all_b.OnClick(sell_all)
		sell_all_b.Tooltip = "Sell All"
		
		-- Buy all button
		local buy_all_b = Button.Create()
		buy_all_b.SetType("simple")
		buy_all_b.SetSprite(SpriteInfo.__new("UI", "plus_all"))
		buy_all_b.SetNonExpanding()
		local buy_all = function() 
			trade.AddToBasket(part, 1000)
			UpdateText()
			UpdateMerchant()
		end
		buy_all_b.OnClick(buy_all)
		buy_all_b.Tooltip = "Buy All"
		
		-- Reset
		local reset_b = Button.Create()
		reset_b.SetType("simple")
		reset_b.SetSprite(SpriteInfo.__new("UI", "reset"))
		reset_b.SetNonExpanding()
		local reset = function() 
			trade.AddToBasket(part, -trade.AmountInBasket(part))
			UpdateText()
			UpdateMerchant()
		end
		reset_b.OnClick(reset)
		reset_b.Tooltip = "Reset"
		
		-- Cost label
		local cost_l = Label.Create()
		cost_l.Text = part.Price
		cost_l.SetMinSize(100, -1)
		
		p.AddChild(l_item)
		p.AddChild(reset_b)
		p.AddChild(l_sell)
		p.AddChild(sell_all_b)
		p.AddChild(sell_b)
		p.AddChild(l_basket)
		p.AddChild(buy_b)
		p.AddChild(buy_all_b)
		p.AddChild(l_buy)
		p.AddChild(cost_l)
	end
	
	reset_f = function()
		trade.Reset()
		rebuildMerchantPanel(UI, verse, this)
		for i, ut in pairs(ut_map) do
			ut()
		end
		UpdateMerchant()
	end
	b_reset.OnClick(reset_f)
	finalize_f = function()
		if trade.FinalizeTransaction() then
			reset_f()
		end
		update_funds_text()
	end
	b_finalize.OnClick(finalize_f)
	
	for part, item in pairs(trade.GetBuyInventory().Get()) do
		AddPartPanel(part, item)
	end
	for part, item in pairs(trade.GetSellInventory().Get()) do
		if not trade.GetBuyInventory().Contains(part) then
		    AddPartPanel(part, item)
		end
	end
end
