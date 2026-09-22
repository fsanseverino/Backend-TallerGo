namespace Backend_TallerGo;

/// Catálogo único de permisos del sistema. El rol ADMIN siempre tiene todos.
public static class PermisosCatalog
{
    public static readonly string[] Todos =
    {
        "clientes:ver", "clientes:crear", "clientes:editar", "clientes:eliminar",
        "vehiculos:ver", "vehiculos:crear", "vehiculos:editar", "vehiculos:eliminar",
        "empleados:ver", "empleados:crear", "empleados:editar", "empleados:eliminar",
        "trabajos:ver", "trabajos:crear", "trabajos:editar", "trabajos:eliminar",
        "presupuestos:ver", "presupuestos:crear", "presupuestos:editar", "presupuestos:eliminar", "presupuestos:convertir",
        "agenda:ver", "agenda:crear", "agenda:editar", "agenda:eliminar",
        "inventario:ver", "inventario:crear", "inventario:editar", "inventario:eliminar",
        "caja:ver", "caja:abrir", "caja:cerrar", "caja:registrar", "caja:cobrar",
        "reportes:ver",
        "catalogos:ver", "catalogos:editar",
        "configuracion:ver", "configuracion:editar",
        "usuarios:ver", "usuarios:editar",
    };

    public static readonly string[] DefaultEncargado =
    {
        "clientes:ver", "clientes:crear", "clientes:editar", "clientes:eliminar",
        "vehiculos:ver", "vehiculos:crear", "vehiculos:editar", "vehiculos:eliminar",
        "empleados:ver",
        "trabajos:ver", "trabajos:crear", "trabajos:editar",
        "presupuestos:ver", "presupuestos:crear", "presupuestos:editar", "presupuestos:convertir",
        "agenda:ver", "agenda:crear", "agenda:editar", "agenda:eliminar",
        "inventario:ver", "inventario:crear", "inventario:editar",
        "caja:ver", "caja:abrir", "caja:cerrar", "caja:registrar", "caja:cobrar",
        "reportes:ver",
        "catalogos:ver",
    };

    public static readonly string[] DefaultOperador =
    {
        "clientes:ver",
        "vehiculos:ver",
        "empleados:ver",
        "trabajos:ver", "trabajos:editar",
        "agenda:ver",
    };
}