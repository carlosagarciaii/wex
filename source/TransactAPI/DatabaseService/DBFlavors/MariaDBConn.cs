using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DatabaseService.DBFlavors;

public class MariaDBConn: IAsyncDisposable
{

    #region Properties

    private int connections = 0;

    #endregion


    #region LifeCycle
        private static SemaphoreSlim _lockObj = new(1, 1);
    private static List<MariaDBConn> _instances { get; set; } = new();
    public ReadOnlyCollection<MariaDBConn> Instances { get => _instances.AsReadOnly(); }



    public async ValueTask DisposeAsync()
    {

    }

    #endregion


    #region Methods

    #endregion



}
