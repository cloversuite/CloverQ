﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace QueueSystem
{

    public enum CallOrderStrategy { DEFAULT }

    /// <summary>
    /// Representa una lista de llamadas ordenada por order de llegada.
    /// Al igual que la lista de miembros MemberList se debería 
    /// implementar una estrategia de ordenamiento (sort) para poder
    /// por ejemplo una lista de llamadas ordenada por prioridad.
    /// Una llamada se debe mantener dentro de la lista hasta que 
    /// conecte con un miembro o abandone la lista por algun motivo, 
    /// por ej:timeout dentro de la lista, el llamante terminó la llamada, 
    /// el llamante oprimió una tecla y se encuentra habilitada la funcionalidad
    /// de abandoar la lista mediante este evento
    /// </summary>
    public class CallList
    {
        ICallOrderStrategy strategy = null;

        /// <summary>
        /// En esta lista se mantiene el orden de llegada de las llamadas.
        /// </summary>
        List<Call> calls = null;

        public CallList() {
            //Creo una lista vacía al comienzo
            calls = new List<Call>();
            //Creo la estrategia de ordenamiento por defetco para las llamadas
            strategy = new CallOrderStrategyDefault(calls);
        }

        public List<Call> Calls {
            get
            {
                return calls;
            }
        }

        /// <summary>
        /// Método que me permite cambiar la estrategia ordenamiento de llamada
        /// </summary>
        /// <param name="strategy"></param>
        public void SetCallOrderSrtategy(ICallOrderStrategy strategy)
        {
            this.strategy = strategy;
            this.strategy.SetCallList(this.calls);
        }

        /// <summary>
        /// Método que me permite cambiar la estrategia ordenamiento de llamada
        /// </summary>
        /// <param name="strategy"></param>
        public void SetCallOrderSrtategy(CallOrderStrategy strategy)
        {
            switch (strategy)
            {
                case CallOrderStrategy.DEFAULT:
                    SetCallOrderSrtategy(new CallOrderStrategyDefault());
                    break;
                default:
                    SetCallOrderSrtategy(new CallOrderStrategyDefault());
                    break;
            }
        }


        /// <summary>
        /// Método que me permite cambiar la estrategia ordenamiento de llamada
        /// </summary>
        /// <param name="strategy"></param>
        public void SetCallOrderSrtategy(string strategy)
        {
            try
            {
                SetCallOrderSrtategy((CallOrderStrategy)Enum.Parse(typeof(CallOrderStrategy), strategy.ToUpper()));
            }
            catch (Exception ex)
            {
                Log.Logger.Debug("Error en SetCallOrderSrtategy: " + ex.Message);
            }
        }

        public void AddCall(Call call) {
            calls.Add(call);
        }

        public void RemoveCall(Call call) {
            //TODO: Revisar esto, lo escribo así mas que nada para expresar lo que se debe hacer
            RemoveCall(call.CallHandlerId);

        }

        public Call RemoveCall(string callHandlerId)
        {
            //TODO: Revisar esto, lo escribo así mas que nada para expresar lo que se debe hacer
            Call callToRemove = calls.Find(c => c.CallHandlerId == callHandlerId);
            if (callToRemove != null)
            {
                //callToRemove.QueueMember = null; //desreferencio
                calls.Remove(callToRemove);
            }
            return callToRemove;
        }

        public Call GetCallById(string callhandlerId) {
            Call call = null;
            foreach (Call c in calls) {
                if (c.CallHandlerId == callhandlerId) {
                    call = c;
                }
            }
            return call;
        }

        /// <summary>
        /// Este métdo debe devolver la siguiente llamada a conectar con un miembro.
        /// Se debe tener en cuenta que en la lista pueden existir llamadas que estan 
        /// intenetando contactar un miembro
        /// </summary>
        /// <returns></returns>
        public Call NextToDispatch() {
            //TODO: implementar la lógica que determina si una llamada esta lista para despacharse
            return strategy.GetNext();
        }

    }
}
