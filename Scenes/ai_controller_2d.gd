extends AIController2D

# 1. Referencia a tu script de C#
@onready var fumiko = get_parent()

# 2. Le pasamos a Python los RayCasts que programamos
func get_obs() -> Dictionary:
	return {"obs": fumiko.GetObservations()}

# 3. Le pasamos el Reward (las galletas)
func get_reward() -> float:
	return fumiko.Reward

# 4. Definimos qué puede hacer la IA (Moverse y Saltar)
func get_action_space() -> Dictionary:
	return {
		"move_and_jump": {"size": 2, "action_type": "continuous"}
	}

# 5. Recibimos las órdenes de Python y las mandamos a Fumiko
func set_action(action) -> void:
	# action[0] será el movimiento X (-1 a 1)
	# action[1] será el salto (si es > 0.5, salta)
	fumiko.AiActions[0] = action["move_and_jump"][0]
	fumiko.AiActions[1] = action["move_and_jump"][1]

# 6. Le decimos a Python si Fumiko murió
func get_done() -> bool:
	return fumiko.isDead # Asegúrate de que isDead sea público en C# o haz un getter