def test_server_module_imports_without_schema_generation_errors() -> None:
    import server

    assert server.mcp is not None
