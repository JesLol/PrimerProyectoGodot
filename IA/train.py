import os
from godot_rl.wrappers.stable_baselines_wrapper import StableBaselinesGodotEnv
from stable_baselines3 import PPO

# 1. Configurar el entorno
env = StableBaselinesGodotEnv(env_path=None)
model_path = "fumiko_ai_model.zip"

# 2. LÓGICA DE MEMORIA (Carga o Crea)
if os.path.exists(model_path):
    print(f"--- Encontrado cerebro previo ({model_path}). Cargando experiencia... ---")
    # Cargamos el modelo existente
    model = PPO.load(model_path, env=env, device="cuda")
else:
    print("--- No hay cerebro previo. Creando uno nuevo desde cero... ---")
    # Creamos uno nuevo si no existe
    model = PPO("MultiInputPolicy", env, verbose=1, device="cuda")

# 3. Entrenar
# reset_num_timesteps=False hace que los pasos se sumen (ej. empezar en 100k y seguir a 200k)
model.learn(total_timesteps=150000, reset_num_timesteps=False, log_interval=1)

# 4. Guardar al terminar
model.save("fumiko_ai_model")
print("--- Entrenamiento guardado exitosamente ---")