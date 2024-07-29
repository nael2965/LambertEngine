#pragma once
#include "ComponentsCommon.h"

namespace lambnert::transform {
	DEFINE_TYPED_ID(transform_id);

	transform_id create_tranform(const init_info& info, game_entity::entity_id entity_id);
}