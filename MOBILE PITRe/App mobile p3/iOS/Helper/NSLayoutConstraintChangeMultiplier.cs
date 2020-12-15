using System;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.Helper
{
    public class NSLayoutConstraintChangeMultiplier
    {
        /// <summary>
        /// Changes the multiplier.
        /// </summary>
        /// <returns> Il nuovo constraint col multipplier cambiato </returns>
        /// <param name="constraint">Constraint. Il constraint di cui cambiare il multiplier </param>
        /// <param name="multiplier">Multiplier. Il nuovo multiplier </param>
        public static NSLayoutConstraint changeMultiplier (NSLayoutConstraint constraint, nfloat multiplier)
        {
            NSLayoutConstraint [] ar = { constraint };
            NSLayoutConstraint.DeactivateConstraints (ar);

            NSLayoutConstraint newConstraint = NSLayoutConstraint.Create (constraint.FirstItem, 
                                                                          constraint.FirstAttribute, 
                                                                          constraint.Relation, 
                                                                          constraint.SecondItem, 
                                                                          constraint.SecondAttribute, 
                                                                          multiplier, 
                                                                          constraint.Constant);

            newConstraint.Priority = constraint.Priority;
            newConstraint.ShouldBeArchived = constraint.ShouldBeArchived;
            newConstraint.SetIdentifier (constraint.GetIdentifier ());

            NSLayoutConstraint [] ar2 = { newConstraint };
            NSLayoutConstraint.ActivateConstraints (ar2);

            return newConstraint;
        }
    }
}