#pragma once
#include "ComponentsCommon.h"

namespace lambert::transform {

	struct init_info
	{
		F32 position[3]{};
		F32 rotation[4]{};
		F32 scale[3]{1.f,1.f,1.f};
	};
	
	component create_transform(const init_info& info, game_entity::entity entity);
	void remove_transform(component c);
}