from fastmcp import FastMCP
from starlette.applications import Starlette
from starlette.routing import Route
from starlette.responses import JSONResponse
from tools.inventory import get_inventory
from tools.recipes import get_recipes
from tools.shopping import get_shopping_list

mcp = FastMCP("cookhomie")
mcp.tool()(get_inventory)
mcp.tool()(get_recipes)
mcp.tool()(get_shopping_list)

mcp_app = mcp.http_app()

async def api_inventory(request):
    try:
        location = request.query_params.get("location")
        result = await get_inventory(location)
        return JSONResponse(result)
    except Exception as e:
        return JSONResponse({"error": str(e)}, status_code=500)

async def api_recipes(request):
    try:
        query = request.query_params.get("query")
        result = await get_recipes(query)
        return JSONResponse(result)
    except Exception as e:
        return JSONResponse({"error": str(e)}, status_code=500)

async def api_shopping(request):
    try:
        result = await get_shopping_list()
        return JSONResponse(result)
    except Exception as e:
        return JSONResponse({"error": str(e)}, status_code=500)

async def homepage(request):
    return JSONResponse({"status": "ok", "service": "cookhomie-mcp"})

app = Starlette(
    routes=[
        Route("/", endpoint=homepage),
        Route("/api/inventory", endpoint=api_inventory),
        Route("/api/recipes", endpoint=api_recipes),
        Route("/api/shopping-list", endpoint=api_shopping),
    ],
    debug=True,
)

app.mount("/mcp", mcp_app)

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)