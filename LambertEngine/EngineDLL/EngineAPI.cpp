#ifndef EDITOR_INTERFACE
#define EDITOR_INTERFACE extern "C" __declspec(dllexport)
#endif  // !EDITOR_INTERFACE

#include "CommonHeaders.h"
#include "Id.h"
#include "../Engine/Components/Entity.h"
#include "../Engine/Components/Transform.h"

using namespace lambert;

EDITOR_INTERFACE id::id_type CreateGameEntity()
{
    
}